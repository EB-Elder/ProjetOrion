using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;

[AlwaysSynchronizeSystem]
//update après que le job TriggerSystem test les collisions entre le joueur et les projo 
[UpdateAfter(typeof(TriggerSystem))]
public class ExplosionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<ExplosionTag>().ForEach((Entity entity) =>
            {

                commandBuffer.DestroyEntity(entity);

        }).Run();

        
       

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;

    }
    
}
