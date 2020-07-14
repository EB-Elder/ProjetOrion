using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

[AlwaysSynchronizeSystem]
public class ForceShieldSystem : JobComponentSystem
{
    [BurstCompile]
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var shield = GetComponentDataFromEntity<ShieldTag>();

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        float input = Input.GetAxis("RightTrigger");

        //pour toutes les entités qui ont un forceshield actif
        Entities.ForEach((Entity e, ref PlayerStatsData playerStatsData, ref ForceShieldTag shieldTag) =>
        {
            //si la gachette est relachée, on enlève le tag de ForceShield
            if(input < 0.5f)
            {
                commandBuffer.RemoveComponent<ForceShieldTag>(e);
            }
            else
            {
                //sinon le shield consomme de la stamina
                playerStatsData.stamina -= 1;
            }

            //si le joueur n'a plus de stamina, le forceshield est désactivé
            if (playerStatsData.stamina <= 0)
            {
                commandBuffer.RemoveComponent<ForceShieldTag>(e);
            }

        }).Run();

        var verif = GetComponentDataFromEntity<ForceShieldTag>();

        Entities.ForEach((Entity e, ref PlayerStatsData playerStatsData) =>
        {
            //si le joueur appuie sur la gachette droite on active le forceshield
            if (input > 0.5f && playerStatsData.stamina > 0)
            {
                //si le joueur n'a pas le tag ForceShield, on l'ajoute
                if (!verif.HasComponent(e))
                {
                    commandBuffer.AddComponent(e, new ForceShieldTag());
                }
            }
            else
            {
                if(playerStatsData.stamina < 100.0f)
                {
                    playerStatsData.stamina += 0.1f;
                }
            }

        }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;
    }
}
