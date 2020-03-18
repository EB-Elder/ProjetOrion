using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Physics
{
    // An event raised when a pair of bodies have collided during solving.
    public struct CollisionEvent
    {
        internal CollisionEventDataRef EventData;
        internal float TimeStep;
        internal Velocity InputVelocityA;
        internal Velocity InputVelocityB;

        public EntityPair Entities { get; internal set; }

        public BodyIndexPair BodyIndices => EventData.Value.BodyIndices;

        public ColliderKeyPair ColliderKeys => EventData.Value.ColliderKeys;

        public float3 Normal => EventData.Value.Normal;

        // Calculate extra details about the collision.
        // Note: Since the solver does not naturally produce this data, it requires some computation.
        public Details CalculateDetails(ref PhysicsWorld physicsWorld)
        {
            int numContactPoints = EventData.Value.NumNarrowPhaseContactPoints;
            var contactPoints = new NativeArray<ContactPoint>(numContactPoints, Allocator.Temp);
            for (int i = 0; i < numContactPoints; i++)
            {
                contactPoints[i] = EventData.Value.AccessContactPoint(i);
            }

            return EventData.Value.CalculateDetails(ref physicsWorld, TimeStep, InputVelocityA, InputVelocityB, contactPoints);
        }

        // Extra details about a collision
        public struct Details
        {
            // Estimated contact point positions (in world space).
            // Use this information about individual contact point positions
            // to apply custom logic, for example looking at the Length
            // to differentiate between vertex (1 point), edge (2 point)
            // or face (3 or more points) collision.
            public NativeArray<float3> EstimatedContactPointPositions;

            // Estimated total impulse applied
            public float EstimatedImpulse;

            // Calculate the average contact point position
            public float3 AverageContactPointPosition
            {
                get
                {
                    var pos = float3.zero;
                    for (int i = 0; i < EstimatedContactPointPositions.Length; i++)
                    {
                        pos += EstimatedContactPointPositions[i];
                    }
                    return pos / EstimatedContactPointPositions.Length;
                }
            }
        }
    }

    // A stream of collision events.
    // This is a value type, which means it can be used in Burst jobs (unlike IEnumerable<CollisionEvent>).
    public struct CollisionEvents /* : IEnumerable<CollisionEvent> */
    {
        //@TODO: Unity should have a Allow null safety restriction
        [NativeDisableContainerSafetyRestriction]
        private readonly NativeStream m_EventDataStream;

        private readonly NativeSlice<RigidBody> m_Bodies;
        private readonly NativeSlice<Velocity> m_InputVelocities;
        private readonly float m_TimeStep;

        internal CollisionEvents(NativeStream eventDataStream, NativeSlice<RigidBody> bodies, NativeSlice<Velocity> inputVelocities, float timeStep)
        {
            m_EventDataStream = eventDataStream;
            m_Bodies = bodies;
            m_InputVelocities = inputVelocities;
            m_TimeStep = timeStep;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_EventDataStream, m_Bodies, m_InputVelocities, m_TimeStep);
        }

        public struct Enumerator /* : IEnumerator<CollisionEvent> */
        {
            private NativeStream.Reader m_Reader;
            private int m_CurrentWorkItem;
            private readonly int m_NumWorkItems;
            private CollisionEventDataRef m_Current;

            private readonly NativeSlice<RigidBody> m_Bodies;
            private readonly NativeSlice<Velocity> m_InputVelocities;
            private readonly float m_TimeStep;

            public CollisionEvent Current
            {
                get => m_Current.Value.CreateCollisionEvent(m_TimeStep, m_Bodies, m_InputVelocities);
            }

            internal Enumerator(NativeStream stream, NativeSlice<RigidBody> bodies, NativeSlice<Velocity> inputVelocities, float timeStep)
            {
                m_Reader = stream.IsCreated ? stream.AsReader() : new NativeStream.Reader();
                m_CurrentWorkItem = 0;
                m_NumWorkItems = stream.IsCreated ? stream.ForEachCount : 0;

                m_Bodies = bodies;
                m_InputVelocities = inputVelocities;
                m_TimeStep = timeStep;

                unsafe
                {
                    m_Current = default;
                }

                AdvanceReader();
            }

            public bool MoveNext()
            {
                if (m_Reader.RemainingItemCount > 0)
                {
                    int currentSize = m_Reader.Read<int>();
                    AdvanceReader();

                    unsafe
                    {
                        m_Current = new CollisionEventDataRef((CollisionEventData*)(m_Reader.ReadUnsafePtr(currentSize)));
                    }

                    AdvanceReader();
                    return true;
                }
                return false;
            }

            private void AdvanceReader()
            {
                while (m_Reader.RemainingItemCount == 0 && m_CurrentWorkItem < m_NumWorkItems)
                {
                    m_Reader.BeginForEachIndex(m_CurrentWorkItem);
                    m_CurrentWorkItem++;
                }
            }
        }
    }

    // An event raised when a pair of bodies have collided during solving.
    struct CollisionEventData
    {
        public BodyIndexPair BodyIndices;
        public ColliderKeyPair ColliderKeys;
        public float3 Normal;

        // The total impulse applied by the solver for this pair
        internal float SolverImpulse;

        // Number of narrow phase contact points
        internal int NumNarrowPhaseContactPoints;

        internal unsafe CollisionEvent CreateCollisionEvent(float timeStep, NativeSlice<RigidBody> bodies, NativeSlice<Velocity> inputVelocities)
        {
            int bodyAIndex = BodyIndices.BodyAIndex;
            int bodyBIndex = BodyIndices.BodyBIndex;
            return new CollisionEvent
            {
                EventData = new CollisionEventDataRef((CollisionEventData*)(UnsafeUtility.AddressOf(ref this))),
                Entities = new EntityPair
                {
                    EntityA = bodies[bodyAIndex].Entity,
                    EntityB = bodies[bodyBIndex].Entity
                },
                TimeStep = timeStep,
                InputVelocityA = bodyAIndex < inputVelocities.Length ? inputVelocities[bodyAIndex] : Velocity.Zero,
                InputVelocityB = bodyBIndex < inputVelocities.Length ? inputVelocities[bodyBIndex] : Velocity.Zero
            };
        }

        internal static int CalculateSize(int numContactPoints)
        {
            return UnsafeUtility.SizeOf<CollisionEventData>() + numContactPoints * UnsafeUtility.SizeOf<ContactPoint>();
        }

        internal unsafe ref ContactPoint AccessContactPoint(int pointIndex)
        {
            byte* ptr = (byte*)UnsafeUtility.AddressOf(ref this);
            ptr += UnsafeUtility.SizeOf<CollisionEventData>() + pointIndex * UnsafeUtility.SizeOf<ContactPoint>();
            return ref UnsafeUtilityEx.AsRef<ContactPoint>(ptr);
        }

        // Calculate extra details about the collision, by re-integrating the leaf colliders to the time of collision
        internal unsafe CollisionEvent.Details CalculateDetails(
            ref PhysicsWorld physicsWorld, float timeStep, Velocity inputVelocityA, Velocity inputVelocityB, NativeArray<ContactPoint> narrowPhaseContactPoints)
        {
            int bodyAIndex = BodyIndices.BodyAIndex;
            int bodyBIndex = BodyIndices.BodyBIndex;
            bool bodyAIsDynamic = bodyAIndex < physicsWorld.MotionVelocities.Length;
            bool bodyBIsDynamic = bodyBIndex < physicsWorld.MotionVelocities.Length;

            MotionVelocity motionVelocityA = bodyAIsDynamic ? physicsWorld.MotionVelocities[bodyAIndex] : MotionVelocity.Zero;
            MotionVelocity motionVelocityB = bodyBIsDynamic ? physicsWorld.MotionVelocities[bodyBIndex] : MotionVelocity.Zero;
            MotionData motionDataA = bodyAIsDynamic ? physicsWorld.MotionDatas[bodyAIndex] : MotionData.Zero;
            MotionData motionDataB = bodyBIsDynamic ? physicsWorld.MotionDatas[bodyBIndex] : MotionData.Zero;

            float estimatedImpulse = SolverImpulse;

            // First calculate minimum time of impact and estimate the impulse
            float toi = timeStep;
            {
                float sumRemainingVelocities = 0.0f;
                float numRemainingVelocities = 0.0f;
                for (int i = 0; i < narrowPhaseContactPoints.Length; i++)
                {
                    var cp = narrowPhaseContactPoints[i];

                    // Collect data for impulse estimation
                    {
                        float3 pointVelA = GetPointVelocity(motionDataA.WorldFromMotion,
                            motionVelocityA.LinearVelocity, motionVelocityA.AngularVelocity, cp.Position + Normal * cp.Distance);
                        float3 pointVelB = GetPointVelocity(motionDataB.WorldFromMotion,
                            motionVelocityB.LinearVelocity, motionVelocityB.AngularVelocity, cp.Position);
                        float projRelVel = math.dot(pointVelB - pointVelA, Normal);
                        if (projRelVel > 0.0f)
                        {
                            sumRemainingVelocities += projRelVel;
                            numRemainingVelocities += 1.0f;
                        }
                    }

                    // Get minimum time of impact
                    {
                        float3 pointVelA = GetPointVelocity(motionDataA.WorldFromMotion,
                            inputVelocityA.Linear, inputVelocityA.Angular, cp.Position + Normal * cp.Distance);
                        float3 pointVelB = GetPointVelocity(motionDataB.WorldFromMotion,
                            inputVelocityB.Linear, inputVelocityB.Angular, cp.Position);
                        float projRelVel = math.dot(pointVelB - pointVelA, Normal);
                        if (projRelVel > 0.0f)
                        {
                            float newToi = math.max(0.0f, cp.Distance / projRelVel);
                            toi = math.min(toi, newToi);
                        }
                        else if (cp.Distance <= 0.0f)
                        {
                            // If in penetration, time of impact is 0 for sure
                            toi = 0.0f;
                        }
                    }
                }

                if (numRemainingVelocities > 0.0f)
                {
                    float sumInvMass = motionVelocityA.InverseMass + motionVelocityB.InverseMass;
                    estimatedImpulse += sumRemainingVelocities / (numRemainingVelocities * sumInvMass);
                }
            }

            // Then, sub-integrate for time of impact and keep contact points closer than hitDistanceThreshold
            int closestContactIndex = -1;
            float minDistance = float.MaxValue;
            {
                int estimatedContactPointCount = 0;
                for (int i = 0; i < narrowPhaseContactPoints.Length; i++)
                {
                    // Estimate new position
                    var cp = narrowPhaseContactPoints[i];
                    {
                        float3 pointVelA = GetPointVelocity(motionDataA.WorldFromMotion, inputVelocityA.Linear, inputVelocityA.Angular, cp.Position + Normal * cp.Distance);
                        float3 pointVelB = GetPointVelocity(motionDataB.WorldFromMotion, inputVelocityB.Linear, inputVelocityB.Angular, cp.Position);
                        float3 relVel = pointVelB - pointVelA;
                        float projRelVel = math.dot(relVel, Normal);

                        // Only sub integrate if approaching, otherwise leave it as is
                        // (it can happen that input velocity was separating but there
                        // still was a collision event - penetration recovery, or other
                        // body pushing in different direction).
                        if (projRelVel > 0.0f)
                        {
                            // Position the point on body A
                            cp.Position += Normal * cp.Distance;

                            // Sub integrate the point
                            cp.Position -= relVel * toi;

                            // Reduce the distance
                            cp.Distance -= projRelVel * toi;
                        }

                        // Filter out contacts that are still too far away
                        if (cp.Distance <= physicsWorld.CollisionWorld.CollisionTolerance)
                        {
                            narrowPhaseContactPoints[estimatedContactPointCount++] = cp;
                        }
                        else if (cp.Distance < minDistance)
                        {
                            minDistance = cp.Distance;
                            closestContactIndex = i;
                        }
                    }
                }

                // If due to estimation of relative velocity no contact points will
                // get closer than the tolerance, we need to export the closest one
                // to make sure at least one contact point is reported.
                if (estimatedContactPointCount == 0)
                {
                    narrowPhaseContactPoints[estimatedContactPointCount++] = narrowPhaseContactPoints[closestContactIndex];
                }

                // Instantiate collision details and allocate memory
                var details = new CollisionEvent.Details
                {
                    EstimatedContactPointPositions = new NativeArray<float3>(estimatedContactPointCount, Allocator.Temp),
                    EstimatedImpulse = estimatedImpulse
                };

                // Fill the contact point positions array
                for (int i = 0; i < estimatedContactPointCount; i++)
                {
                    details.EstimatedContactPointPositions[i] = narrowPhaseContactPoints[i].Position;
                }

                return details;
            }
        }

        private static float3 GetPointVelocity(RigidTransform worldFromMotion, float3 linVel, float3 angVel, float3 point)
        {
            float3 angularVelocity = math.rotate(worldFromMotion, angVel);
            float3 linearVelocity = math.cross(angularVelocity, point - worldFromMotion.pos);
            return linVel + linearVelocity;
        }
    }

    // Wraps a pointer to CollisionEventData.
    // Used in enumerator for collision events.
    struct CollisionEventDataRef
    {
        private unsafe CollisionEventData* m_CollisionEventData;

        public unsafe CollisionEventDataRef(CollisionEventData* collisionEventData)
        {
            m_CollisionEventData = collisionEventData;
        }

        public unsafe ref CollisionEventData Value => ref UnsafeUtilityEx.AsRef<CollisionEventData>(m_CollisionEventData);
    }
}
