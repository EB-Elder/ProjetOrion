using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;

[AlwaysSynchronizeSystem]
//update après que le job TriggerSystem test les collisions entre le joueur et les projo 
[UpdateAfter(typeof(TriggerSystem))]
public class ExplosionSystem : JobComponentSystem
{

    private bool explosion;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<ExplosionTag>().ForEach((Entity entity) =>
            {

                commandBuffer.DestroyEntity(entity);
                

        }).Run();

        Entities.ForEach((Entity e, ref PlayerStatsData playerStatsData, ref HitTag hitTag) =>
        {

            playerStatsData.Health = playerStatsData.Health - 50;
            commandBuffer.RemoveComponent<HitTag>(e);

            if (playerStatsData.Health <= 0)
            {
                commandBuffer.AddComponent(e, new DeadTag());

            }



        }).Run();







        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;

    }
    
}
