using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

[AlwaysSynchronizeSystem]
//update après que le job TriggerSystem test les collisions entre le joueur et les projo 
//[UpdateAfter(typeof(HitBossCollisionSystem))]
public class PlayerMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        float deltaTime = Time.DeltaTime;

        float2 curInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        // ref veut dire qu'on peut à la fois lire et écrire dans Vel
        // In indique qu'on a que les droits de lecture sur 
        Entities.ForEach((ref PhysicsVelocity vel, in PlayerStatsData playerStatsData) =>
        {


            // La variable qui va contenir la nouvelle position du joueur
            float2 newVel = vel.Linear.xz;
            
            newVel += curInput * playerStatsData.movementSpeed * deltaTime;


            //La position du joueur mise à jour
            vel.Linear.xz = newVel;

        }).Run();

        return default;

    }

}
