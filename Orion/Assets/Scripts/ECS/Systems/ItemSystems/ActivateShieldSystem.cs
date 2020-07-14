using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

[UpdateAfter(typeof(ForceShieldSystem))]
public class ActivateShieldSystem : JobComponentSystem
{
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var query = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>(), ComponentType.ReadOnly<ForceShieldTag>());
        var res = query.ToEntityArray(Allocator.TempJob);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        //si on a un joueur avec un forceshield, on active l'entité bouclier
        if (res.Length > 0)
        {
            Entities.ForEach((Entity e, in ShieldTag tag, in Disabled d) =>
            {
                commandBuffer.RemoveComponent<Disabled>(e);

            }).Run();
        }
        else
        {
            //si on a pas de joueur avec un forceshield, on désactive l'entité bouclier
            Entities.ForEach((Entity e, in ShieldTag tag) =>
            {
                commandBuffer.AddComponent<Disabled>(e);

            }).Run();
        }

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        res.Dispose();
        return default;
    }
}
