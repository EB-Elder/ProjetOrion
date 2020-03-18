using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Assertions;
using static Unity.Physics.Math;

namespace Unity.Physics
{
    public enum ConstraintType : byte
    {
        Linear,
        Angular
    }

    // A linear or angular constraint in 1, 2, or 3 dimensions.
    public struct Constraint : IEquatable<Constraint>
    {
        // TODO think more about these
        // Current values give tau = 0.6 damping = 0.99 at 50hz
        // The values are huge and we can't get damping = 1 -- a stiff constraint is the limit of a damped spring as spring params go to infinity.
        public const float DefaultSpringFrequency = 61950.977267809007887192914302327f;
        public const float DefaultSpringDamping = 2530.12155587434178122630287018f;

        public bool3 ConstrainedAxes;
        public ConstraintType Type;

        public float Min;
        public float Max;
        public float SpringFrequency;
        public float SpringDamping;

        // Number of affected degrees of freedom.  1, 2, or 3.
        internal int Dimension => math.select(math.select(2, 1, ConstrainedAxes.x ^ ConstrainedAxes.y ^ ConstrainedAxes.z), 3, math.all(ConstrainedAxes));

        // Selects the free axis from a constraint with Dimension == 1
        internal int FreeAxis2D
        {
            get
            {
                Assert.IsTrue(Dimension == 2);
                return math.select(2, math.select(0, 1, ConstrainedAxes[0]), ConstrainedAxes[2]);
            }
        }

        // Selects the constrained axis from a constraint with Dimension == 2
        internal int ConstrainedAxis1D
        {
            get
            {
                Assert.IsTrue(Dimension == 1);
                return math.select(math.select(1, 0, ConstrainedAxes[0]), 2, ConstrainedAxes[2]);
            }
        }

        #region Common linear constraints

        /// <summary>
        /// Constrains linear motion about all three axes to zero.
        /// </summary>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint BallAndSocket(float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            return new Constraint
            {
                ConstrainedAxes = new bool3(true),
                Type = ConstraintType.Linear,
                Min = 0.0f,
                Max = 0.0f,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains linear motion about all three axes within the specified range.
        /// </summary>
        /// <param name="distanceRange">The minimum required distance and maximum possible distance between the constrained bodies.</param>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint LimitedDistance(FloatRange distanceRange, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            return new Constraint
            {
                ConstrainedAxes = new bool3(true),
                Type = ConstraintType.Linear,
                Min = distanceRange.Min,
                Max = distanceRange.Max,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains linear motion about two axes within the specified range. Movement about the third is unrestricted.
        /// </summary>
        /// <param name="freeAxis">The axis along which the bodies may freely translate.</param>
        /// <param name="distanceRange">
        /// The minimum required distance and maximum possible distance between the constrained bodies about the two constrained axes.
        /// A minimum value of zero produces a cylindrical range of motion, while a minimum value greater than zero results in a tube-shaped range of motion.
        /// </param>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint Cylindrical(int freeAxis, FloatRange distanceRange, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            Assert.IsTrue(freeAxis >= 0 && freeAxis <= 2);
            return new Constraint
            {
                ConstrainedAxes = new bool3(freeAxis != 0, freeAxis != 1, freeAxis != 2),
                Type = ConstraintType.Linear,
                Min = distanceRange.Min,
                Max = distanceRange.Max,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains linear motion about one axis within the specified range. Movement about the other two is unrestricted.
        /// </summary>
        /// <param name="limitedAxis">The axis along which the bodies' translation is restricted.</param>
        /// <param name="distanceRange">
        /// The minimum required distance and maximum possible distance between the constrained bodies about the constrained axis.
        /// Identical minimum and maximum values result in a plane, while different values constrain the bodies between two parallel planes.
        /// </param>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint Planar(int limitedAxis, FloatRange distanceRange, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            Assert.IsTrue(limitedAxis >= 0 && limitedAxis <= 2);
            return new Constraint
            {
                ConstrainedAxes = new bool3(limitedAxis == 0, limitedAxis == 1, limitedAxis == 2),
                Type = ConstraintType.Linear,
                Min = distanceRange.Min,
                Max = distanceRange.Max,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        #endregion

        #region Common angular constraints

        /// <summary>
        /// Constrains angular motion about all three axes to zero.
        /// </summary>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint FixedAngle(float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            return new Constraint
            {
                ConstrainedAxes = new bool3(true),
                Type = ConstraintType.Angular,
                Min = 0.0f,
                Max = 0.0f,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains angular motion about two axes to zero. Rotation around the third is unrestricted.
        /// </summary>
        /// <param name="freeAxis">The axis around which the bodies may freely rotate.</param>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint Hinge(int freeAxis, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            Assert.IsTrue(freeAxis >= 0 && freeAxis <= 2);
            return new Constraint
            {
                ConstrainedAxes = new bool3(freeAxis != 0, freeAxis != 1, freeAxis != 2),
                Type = ConstraintType.Angular,
                Min = 0.0f,
                Max = 0.0f,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains angular motion about two axes within the specified range. Rotation around the third is unrestricted.
        /// </summary>
        /// <param name="freeAxis">The axis specifying the height of the cone within which the bodies may rotate.</param>
        /// <param name="angularRange">
        /// The minimum required angle and maximum possible angle between the free axis and its bind pose orientation.
        /// A minimum value of zero produces a conical range of motion, while a minimum value greater than zero results in motion restricted to the intersection of the inner and outer cones.
        /// </param>
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint Cone(int freeAxis, FloatRange angularRange, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            Assert.IsTrue(freeAxis >= 0 && freeAxis <= 2);
            return new Constraint
            {
                ConstrainedAxes = new bool3(freeAxis != 0, freeAxis != 1, freeAxis != 2),
                Type = ConstraintType.Angular,
                Min = angularRange.Min,
                Max = angularRange.Max,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        /// <summary>
        /// Constrains angular motion about about one axis within the specified range.
        /// </summary>
        /// <param name="limitedAxis">The axis around which the bodies' rotation is restricted.</param>
        /// <param name="angularRange">The minimum required angle and maximum possible angle of rotation between the constrained bodies around the constrained axis.
        /// <param name="springFrequency"></param>
        /// <param name="springDamping"></param>
        public static Constraint Twist(int limitedAxis, FloatRange angularRange, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping)
        {
            Assert.IsTrue(limitedAxis >= 0 && limitedAxis <= 2);
            return new Constraint
            {
                ConstrainedAxes = new bool3(limitedAxis == 0, limitedAxis == 1, limitedAxis == 2),
                Type = ConstraintType.Angular,
                Min = angularRange.Min,
                Max = angularRange.Max,
                SpringFrequency = springFrequency,
                SpringDamping = springDamping
            };
        }

        #endregion

        public bool Equals(Constraint other)
        {
            return ConstrainedAxes.Equals(other.ConstrainedAxes)
                && Type == other.Type
                && Min.Equals(other.Min)
                && Max.Equals(other.Max)
                && SpringFrequency.Equals(other.SpringFrequency)
                && SpringDamping.Equals(other.SpringDamping);
        }

        public override bool Equals(object obj) => obj is Constraint other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)math.hash(new uint3x2(
                    new uint3((uint)Type, (uint)Min.GetHashCode(), (uint)Max.GetHashCode()),
                    new uint3(math.hash(ConstrainedAxes), (uint)SpringFrequency.GetHashCode(), (uint)SpringDamping.GetHashCode())
                ));
            }
        }

        #region Obsolete

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method has been deprecated. Use LimitedDistance() instead. (RemovedAfter 2020-05-21)")]
        public static Constraint StiffSpring(float minDistance, float maxDistance, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping) =>
            LimitedDistance(new FloatRange(minDistance, maxDistance), springFrequency, springDamping);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static Constraint Cylindrical(int freeAxis, float minDistance, float maxDistance, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping) =>
            Cylindrical(freeAxis, new FloatRange(minDistance, maxDistance), springFrequency, springDamping);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static Constraint Planar(int limitedAxis, float minDistance, float maxDistance, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping) =>
            Planar(limitedAxis, new FloatRange(minDistance, maxDistance), springFrequency, springDamping);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static Constraint Cone(int freeAxis, float minAngle, float maxAngle, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping) =>
            Cone(freeAxis, new FloatRange(minAngle, maxAngle), springFrequency, springDamping);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static Constraint Twist(int limitedAxis, float minAngle, float maxAngle, float springFrequency = DefaultSpringFrequency, float springDamping = DefaultSpringDamping) =>
            Twist(limitedAxis, new FloatRange(minAngle, maxAngle), springFrequency, springDamping);

        #endregion
    }

    /// <summary>
    /// A target in the space of a rigid body that will align with a corresponding target in the space of the other body to which it is joined.
    /// </summary>
    public struct JointFrame : IEquatable<JointFrame>
    {
        /// <summary>
        /// The bind pose anchor or target position of the joint in the space of its rigid body.
        /// </summary>
        public float3 Position;
        /// <summary>
        /// The bind pose orientation of the joint's x-axis in the space of its rigid body.
        /// </summary>
        public float3 Axis;
        /// <summary>
        /// The bind pose orientation of the joint's y-axis in the space of its rigid body.
        /// </summary>
        public float3 PerpendicularAxis;

        public JointFrame(RigidTransform transform)
        {
            Position = transform.pos;
            var rotation = new float3x3(transform.rot);
            Axis = rotation.c0;
            PerpendicularAxis = rotation.c1;
        }

        static readonly float3 k_DefaultAxis = new float3(1f, 0f, 0f);
        static readonly float3 k_DefaultPerpendicular = new float3(0f, 1f, 0f);

        public static readonly JointFrame Identity =
            new JointFrame { Axis = k_DefaultAxis, PerpendicularAxis = k_DefaultPerpendicular };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RigidTransform AsRigidTransform() => new RigidTransform(ValidateAxes(), Position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal MTransform AsMTransform() => new MTransform(ValidateAxes(), Position);

        internal float3x3 ValidateAxes()
        {
            var axis = math.select(k_DefaultAxis, Axis, math.lengthsq(Axis) > 0f);
            var perpendicular =
                math.select(k_DefaultPerpendicular, PerpendicularAxis, math.lengthsq(PerpendicularAxis) > 0f);
            return math.orthonormalize(new float3x3(axis, perpendicular, default));
        }

        public bool Equals(JointFrame other) =>
            Position.Equals(other.Position)
            && Axis.Equals(other.Axis)
            && PerpendicularAxis.Equals(other.PerpendicularAxis);

        public override bool Equals(object obj) => obj is JointFrame other && Equals(other);

        public override int GetHashCode() => unchecked((int)math.hash(new float3x3(Position, Axis, PerpendicularAxis)));

        public override string ToString() =>
            $"JointFrame {{ Axis = {Axis}, PerpendicularAxis = {PerpendicularAxis}, Position = {Position} }}";

        public static implicit operator JointFrame (RigidTransform transform) => new JointFrame(transform);
    }

    // A set of constraints on the relative motion between a pair of rigid bodies.
    // Warning: This is just the header, the joint's variable sized data follows it in memory.
    // Therefore this struct must always be passed by reference, never by value.
    public struct JointData
    {
        // Transform from joint definition space to body A space
        public MTransform AFromJoint
        {
            get => m_AFromJoint;
            set
            {
                if (!m_AFromJoint.Equals(value))
                {
                    m_AFromJoint = value;
                    Constraints.m_Version++;
                }
            }
        }
        private MTransform m_AFromJoint;

        // Transform from joint definition space to body B space
        public MTransform BFromJoint
        {
            get => m_BFromJoint;
            set
            {
                if (!m_BFromJoint.Equals(value))
                {
                    m_BFromJoint = value;
                    Constraints.m_Version++;
                }
            }
        }
        private MTransform m_BFromJoint;

        // Array of individual axis constraints
        public ConstraintsBlob Constraints;

        // Version number, incremented whenever the joint data has changed
        public byte Version => Constraints.m_Version;


        // Container for blob array of constraints
        public struct ConstraintsBlob
        {
            internal BlobArray m_BlobArray;
            internal byte m_Version;

            BlobArray.Accessor<Constraint> Accessor => new BlobArray.Accessor<Constraint>(ref m_BlobArray);

            public int Length => m_BlobArray.Length;

            // Enumerator for the constraints, allowing use of foreach()
            public BlobArray.Accessor<Constraint>.Enumerator GetEnumerator() => Accessor.GetEnumerator();

            // Indexer for the constraints
            // TODO: Add accessor for the constraints based on type? e.g. [LimitedHinge.Axis]
            [System.Runtime.CompilerServices.IndexerName("Constraints")]
            public Constraint this[int index]
            {
                get => Accessor[index];
                set
                {
                    if (!value.Equals(Accessor[index]))
                    {
                        Accessor[index] = value;
                        m_Version++;
                    }
                }
            }
        }


        // Create a joint asset with the given constraints
        public static unsafe BlobAssetReference<JointData> Create(
            JointFrame bodyAFromJoint, JointFrame bodyBFromJoint, NativeArray<Constraint> constraints
        )
        {
            // Allocate
            int totalSize = sizeof(JointData) + sizeof(Constraint) * constraints.Length;
            JointData* jointData = (JointData*)UnsafeUtility.Malloc(totalSize, 16, Allocator.Temp);
            UnsafeUtility.MemClear(jointData, totalSize);

            // Initialize
            {
                jointData->AFromJoint = bodyAFromJoint.AsMTransform();
                jointData->BFromJoint = bodyBFromJoint.AsMTransform();
                jointData->Constraints.m_Version = 1;

                byte* end = (byte*)jointData + sizeof(JointData);
                jointData->Constraints.m_BlobArray.Offset = UnsafeEx.CalculateOffset(end, ref jointData->Constraints.m_BlobArray);
                jointData->Constraints.m_BlobArray.Length = constraints.Length;

                for (int i = 0; i < constraints.Length; i++)
                {
                    jointData->Constraints[i] = constraints[i];
                }
            }

            var blob = BlobAssetReference<JointData>.Create(jointData, totalSize);

            UnsafeUtility.Free(jointData, Allocator.Temp);

            return blob;
        }

        #region Common joint descriptions

        /// <summary>
        /// Creates a joint constraining the point on body A to the point on body B, about which the bodies can freely rotate.
        /// </summary>
        /// <param name="anchorA">Specifies the anchor point in the space of body A.</param>
        /// <param name="anchorB">Specifies the target point in the space of body B.</param>
        public static BlobAssetReference<JointData> CreateBallAndSocket(float3 anchorA, float3 anchorB)
        {
            var jointFrameA = JointFrame.Identity;
            jointFrameA.Position = anchorA;
            var jointFrameB = JointFrame.Identity;
            jointFrameB.Position = anchorB;
            return Create(
                jointFrameA,
                jointFrameB,
                new NativeArray<Constraint>(1, Allocator.Temp)
                {
                    [0] = Constraint.BallAndSocket()
                }
            );
        }

        /// <summary>
        /// Creates a joint constraining the point on body A to the point on body B within the specified permissible linear distance range.
        /// </summary>
        /// <param name="anchorA">Specifies the anchor point in the space of body A.</param>
        /// <param name="anchorB">Specifies the target point in the space of body B.</param>
        /// <param name="distanceRange">The minimum required distance and maximum possible distance between the two anchor points.</param>
        public static BlobAssetReference<JointData> CreateLimitedDistance(float3 anchorA, float3 anchorB, FloatRange distanceRange)
        {
            var jointFrameA = JointFrame.Identity;
            jointFrameA.Position = anchorA;
            var jointFrameB = JointFrame.Identity;
            jointFrameB.Position = anchorB;
            return Create(
                jointFrameA,
                jointFrameB,
                new NativeArray<Constraint>(1, Allocator.Temp)
                {
                    [0] = Constraint.LimitedDistance(distanceRange)
                }
            );
        }

        /// <summary>
        /// Creates a joint constraining an anchor on body A to a target point on body B, where the bodies can translate within a tube-shaped volume.
        /// The tube is oriented down the length of the vector where their axes align.
        /// </summary>
        /// <param name="bodyAFromJoint">Specifies the anchor point and axis of rotation in the space of body A.</param>
        /// <param name="bodyBFromJoint">Specifies the target point and axis of alignment in the space of body B.</param>
        /// <param name="distanceOnAxis">The minimum required and maximum possible distance between the two anchor points along their aligned axes.</param>
        /// <param name="distanceFromAxis">The minimum required and maximum possible distance between the two anchor points along a radius of the cylinder within which they can move.</param>
        public static BlobAssetReference<JointData> CreatePrismatic(
            JointFrame bodyAFromJoint, JointFrame bodyBFromJoint, FloatRange distanceOnAxis, FloatRange distanceFromAxis
        )
        {
            return Create(
                bodyAFromJoint,
                bodyBFromJoint,
                new NativeArray<Constraint>(3, Allocator.Temp)
                {
                    [0] = Constraint.FixedAngle(),
                    [1] = Constraint.Planar(0, distanceOnAxis),
                    [2] = Constraint.Cylindrical(0, distanceFromAxis)
                }            
            );
        }

        /// <summary>
        /// Creates a joint constraining an anchor point on body A to a target point on body B, where each body can rotate freely about the vector where their axes align.
        /// The perpendicular axes of each joint frame have no effect.
        /// </summary>
        /// <param name="bodyAFromJoint">Specifies the anchor point and axis of rotation in the space of body A.</param>
        /// <param name="bodyBFromJoint">Specifies the target point and axis of alignment in the space of body B.</param>
        public static BlobAssetReference<JointData> CreateHinge(JointFrame bodyAFromJoint, JointFrame bodyBFromJoint)
        {
            CalculatePerpendicularNormalized(bodyAFromJoint.Axis, out bodyAFromJoint.PerpendicularAxis, out _);
            CalculatePerpendicularNormalized(bodyBFromJoint.Axis, out bodyBFromJoint.PerpendicularAxis, out _);
            
            return Create(
                bodyAFromJoint,
                bodyBFromJoint,
                new NativeArray<Constraint>(2, Allocator.Temp)
                {
                    [0] = Constraint.Hinge(0),
                    [1] = Constraint.BallAndSocket()
                }            
            );
        }

        /// <summary>
        /// Creates a joint constraining an anchor point on body A to the target point on body B, where each body can rotate within a limited range about the vector where their axes align.
        /// The perpendicular axes of each joint frame are used as a reference point for the range of motion.
        /// </summary>
        /// <param name="bodyAFromJoint">Specifies the anchor point, axis of rotation, and rest orientation in the space of body A.</param>
        /// <param name="bodyBFromJoint">Specifies the target point, axis of alignment, and reference orientation in the space of body B.</param>
        /// <param name="angularRange">The minimum required and maximum possible angle of rotation about the aligned axes.</param>
        public static BlobAssetReference<JointData> CreateLimitedHinge(JointFrame bodyAFromJoint, JointFrame bodyBFromJoint, FloatRange angularRange)
        {
            return Create(
                bodyAFromJoint,
                bodyBFromJoint,
                new NativeArray<Constraint>(3, Allocator.Temp)
                {
                    [0] = Constraint.Twist(0, angularRange),
                    [1] = Constraint.Hinge(0),
                    [2] = Constraint.BallAndSocket()
                }
            );
        }

        /// <summary>
        /// Creates a pair of joints constraining an anchor point on body A to a target point on body B, with a complex angular range of motion suitable for multi-axial joints on characters.
        /// The primary axis of the joined bodies can rotate within a range of motion defined by the maximum cone angle, minus the intersection with a pair of cones along the perpendicular axis.
        /// The bodies may also twist about the primary axis within this range.
        /// </summary>
        /// <param name="bodyAFromJoint">Specifies the anchor point, axis of rotation, and rest orientation in the space of body A.</param>
        /// <param name="bodyBFromJoint">Specifies the target point, axis of alignment, and reference orientation in the space of body B.</param>
        /// <param name="maxConeAngle">Half angle of the primary cone, which defines the maximum possible range of motion in which the primary axis is restricted.</param>
        /// <param name="angularPlaneRange">The range of angular motion defining the cones perpendicular to the primary cone, between which the primary axis may swing. This range may be asymmetrical.</param>
        /// <param name="angularTwistRange">The range of angular motion for twisting around the primary axis within the region defined by the primary and perpendicular cones. This range is usually symmetrical.</param>
        /// <param name="jointData0">The first joint in the system.</param>
        /// <param name="jointData1">The second joint in the system.</param>
        public static void CreateRagdoll(
            JointFrame bodyAFromJoint, JointFrame bodyBFromJoint,
            float maxConeAngle, FloatRange angularPlaneRange, FloatRange angularTwistRange,
            out BlobAssetReference<JointData> jointData0, out BlobAssetReference<JointData> jointData1
        )
        {
            // First joint data: twist and primary cone
            jointData0 = Create(
                bodyAFromJoint,
                bodyBFromJoint,
                new NativeArray<Constraint>(2, Allocator.Temp)
                {
                   [0] = Constraint.Twist(0, angularTwistRange),
                   [1] = Constraint.Cone(0, new FloatRange(0f, maxConeAngle))
                }            
            );

            // Second joint data: perpendicular cone and ball socket
            var transformB = bodyBFromJoint.AsMTransform();
            jointData1 = Create(
                bodyAFromJoint,
                new JointFrame
                {
                    Axis = transformB.Rotation.c1,
                    PerpendicularAxis = transformB.Rotation.c0,
                    Position = transformB.Translation
                },
                new NativeArray<Constraint>(2, Allocator.Temp)
                {
                    [0] = Constraint.Cone(0, angularPlaneRange),
                    [1] = Constraint.BallAndSocket()
                }
            );
        }

        /// <summary>
        /// Creates a joint constraining the position and orientation on body A to the target position and orientation on body B.
        /// </summary>
        /// <param name="bodyAFromJoint">Specifies a reference point and orientation in the space of body A.</param>
        /// <param name="bodyBFromJoint">Specifies a target point and orientation in the space of body B.</param>
        public static BlobAssetReference<JointData> CreateFixed(JointFrame bodyAFromJoint, JointFrame bodyBFromJoint)
        {
            return Create(
                bodyAFromJoint,
                bodyBFromJoint,
                new NativeArray<Constraint>(2, Allocator.Temp)
                {
                    [0] = Constraint.BallAndSocket(),
                    [1] = Constraint.FixedAngle()
                }
            );
        }

        #endregion

        #region Obsolete

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Create has been deprecated. Use the variant passing JointFrame and NativeArray<Constraint> instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> Create(MTransform bodyAFromJoint, MTransform bodyBFromJoint, Constraint[] constraints) =>
            Create(
                new RigidTransform(bodyAFromJoint.Rotation, bodyAFromJoint.Translation),
                new RigidTransform(bodyBFromJoint.Rotation, bodyBFromJoint.Translation),
                new NativeArray<Constraint>(constraints, Allocator.Temp)
            );

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing JointFrame instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> CreateFixed(float3 positionAinA, float3 positionBinB, quaternion orientationAinA, quaternion orientationBinB) =>
            CreateFixed(new RigidTransform(orientationAinA, positionAinA), new RigidTransform(orientationBinB, positionBinB));

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing JointFrame instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> CreateHinge(float3 positionAinA, float3 positionBinB, float3 axisInA, float3 axisInB) =>
            CreateHinge(new JointFrame { Position = positionAinA, Axis = axisInA }, new JointFrame { Position = positionBinB, Axis = axisInB });

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing JointFrame and Math.FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> CreateLimitedHinge(
            float3 positionAinA, float3 positionBinB, float3 axisInA, float3 axisInB, float3 perpendicularInA, float3 perpendicularInB, float minAngle, float maxAngle
        ) =>
            CreateLimitedHinge(
                new JointFrame { Axis = axisInA, PerpendicularAxis = perpendicularInA, Position = positionAinA },
                new JointFrame { Axis = axisInB, PerpendicularAxis = perpendicularInB, Position = positionBinB },
                new FloatRange(minAngle, maxAngle)
            );

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing JointFrame and Math.FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> CreatePrismatic(
            float3 positionAinA, float3 positionBinB, float3 axisInA, float3 axisInB, float3 perpendicularAxisInA, float3 perpendicularAxisInB,
            float minDistanceOnAxis, float maxDistanceOnAxis, float minDistanceFromAxis, float maxDistanceFromAxis
        ) =>
            CreatePrismatic(
                new JointFrame { Axis = axisInA, PerpendicularAxis = perpendicularAxisInA, Position = positionAinA },
                new JointFrame { Axis = axisInB, PerpendicularAxis = perpendicularAxisInB, Position = positionBinB },
                new FloatRange(minDistanceOnAxis, maxDistanceOnAxis),
                new FloatRange(minDistanceFromAxis, maxDistanceFromAxis)
            );

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This signature has been deprecated. Use the variant passing JointFrame and Math.FloatRange instead. (RemovedAfter 2020-05-21)")]
        public static void CreateRagdoll(float3 positionAinA, float3 positionBinB,
            float3 twistAxisInA, float3 twistAxisInB, float3 perpendicularAxisInA, float3 perpendicularAxisInB,
            float maxConeAngle, float minPerpendicularAngle, float maxPerpendicularAngle, float minTwistAngle, float maxTwistAngle,
            out BlobAssetReference<JointData> jointData0, out BlobAssetReference<JointData> jointData1
        ) =>
            CreateRagdoll(
                new JointFrame{ Axis = twistAxisInA, PerpendicularAxis = perpendicularAxisInA, Position = positionAinA },
                new JointFrame{ Axis = twistAxisInB, PerpendicularAxis = perpendicularAxisInB, Position = positionBinB },
                maxConeAngle,
                new FloatRange(minPerpendicularAngle, maxPerpendicularAngle),
                new FloatRange(minTwistAngle, maxTwistAngle),
                out jointData0,
                out jointData1
            );

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method been deprecated. Use CreateLimitedDistance() instead. (RemovedAfter 2020-05-21)")]
        public static BlobAssetReference<JointData> CreateStiffSpring(float3 anchorA, float3 anchorB, float minDistance, float maxDistance) =>
            CreateLimitedDistance(anchorA, anchorB, new FloatRange(minDistance, maxDistance));

        #endregion
    }

    // A runtime joint instance, attached to specific rigid bodies
    public struct Joint
    {
        public BlobAssetReference<JointData> JointData;
        public BodyIndexPair BodyPair;
        public int EnableCollision; // If non-zero, allows these bodies to collide

        // The entity that contained the component which created this joint
        // Note, this isn't necessarily an entity with a rigid body, as a pair
        // of bodies can have an arbitrary number of constraints, but only one
        // instance of a particular component type per entity
        public Entity Entity;
    }
}
