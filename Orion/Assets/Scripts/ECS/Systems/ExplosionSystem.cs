using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;
using Unity.Burst;

[AlwaysSynchronizeSystem]
//update après que le job TriggerSystem test les collisions entre le joueur et les projo 
[UpdateAfter(typeof(TriggerSystem))]
public class ExplosionSystem : JobComponentSystem
{ 
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<ExplosionTag>().ForEach((Entity entity) =>
        {
            commandBuffer.DestroyEntity(entity);
        }).Run();

        Entities.ForEach((Entity e, ref PlayerStatsData playerStatsData, ref HitTag hitTag) =>
        {
            
            playerStatsData.Health = playerStatsData.Health - hitTag.damage;
            commandBuffer.RemoveComponent<HitTag>(e);

            if (playerStatsData.Health <= 0)
            {
                commandBuffer.AddComponent(e, new DeadTag());
            }

        }).Run();

        Entities.ForEach((Entity e, ref BossStats bossStatsData, ref HitTag hitTag) =>
        {
            bossStatsData.health = bossStatsData.health - hitTag.damage;
            commandBuffer.RemoveComponent<HitTag>(e);

            if (bossStatsData.health <= 0)
            {
                commandBuffer.AddComponent(e, new DeadTag());

            }

        }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;
    }
}
