using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Burst;

[AlwaysSynchronizeSystem]
//update après que le job TriggerSystem test les collisions entre le joueur et les projo 
[UpdateAfter(typeof(HitBossCollisionSystem))]
public class PlayerMovementSystem : JobComponentSystem
{
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        //float2 curInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float f = -Input.GetAxis("LeftJoystickY");
        float r = Input.GetAxis("LeftJoystickX");
        float rotateX = Input.GetAxis("RightJoystickX");

        // ref veut dire qu'on peut à la fois lire et écrire dans Vel
        // In indique qu'on a que les droits de lecture sur 
        Entities.ForEach((ref Translation trans, ref PhysicsVelocity vel, ref Rotation rot, in PlayerStatsData playerStatsData) =>
        {
            //rotation
            Vector3 up = new Quaternion(rot.Value.value.x, rot.Value.value.y, rot.Value.value.z, rot.Value.value.w) * new Vector3(0.0f, 1.0f, 0.0f);
            up.Normalize();
            Quaternion rotation = new Quaternion(up.x, up.y, up.z, playerStatsData.rotationSpeed);
            float4 quaterRot = new float4(rotation.x, rotation.y, rotation.z, rotation.w);
            rot.Value = math.mul(
                    math.normalize(rot.Value),
                    quaternion.AxisAngle(up, playerStatsData.rotationSpeed * deltaTime * rotateX));

            //déplacement
            Vector3 right = new Quaternion(rot.Value.value.x, rot.Value.value.y, rot.Value.value.z, rot.Value.value.w) * new Vector3(1.0f, 0.0f, 0.0f);
            right.Normalize();

            float3 vectorR = new float3(right);

            vel.Linear.xyz += playerStatsData.movementSpeed * deltaTime * (f * math.forward(rot.Value) + r * vectorR);

        }).Run();

        return default;

    }

}
