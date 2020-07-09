/*using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;
using UnityEngine;

public class TriggerSystem : JobComponentSystem
{

    private BeginInitializationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TriggerJob triggerJob = new TriggerJob {

            playerEntity = GetComponentDataFromEntity<PlayerStatsData>(),
            explosivEntities = GetComponentDataFromEntity<ExplosionTag>(),
            projectileEntities = GetComponentDataFromEntity<GrenadeInfernaleTag>(),
            playerHit = GetComponentDataFromEntity<HitTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer()

        };

        

        return triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

    }

    private struct TriggerJob : ITriggerEventsJob
  
    {

        // Le composant qui sert à identifier le joueur
        public ComponentDataFromEntity<PlayerStatsData> playerEntity;

        // Le composant qui sert à identifier les boules qui doivent exploser
        [ReadOnly] public ComponentDataFromEntity<ExplosionTag> explosivEntities;
        
        // Le composant qui sert à marque un joueur touché
        [ReadOnly] public ComponentDataFromEntity<HitTag> playerHit;

        // Le composant qui va servire à vérifier qu'on entre bien en contacte avec une boule d'énergie générée par le boss
        [ReadOnly] public ComponentDataFromEntity<GrenadeInfernaleTag> projectileEntities;

        public EntityCommandBuffer commandBuffer;

        public void Execute (TriggerEvent triggerEvent) {

            
            TestEntityTrigger(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB);
            TestEntityTrigger(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            

        }


        // On test si c'est bien un joueur qui entre en collision avec une sphère susceptible d'exploser au contact
        // Si oui, on marque la sphère comme devant exploser

        // Cette fonction est utilisée juste au dessus dans le corp du job : la fonction execute
        private void TestEntityTrigger(Entity entity1, Entity entity2)
        {
            if (playerEntity.HasComponent(entity1))
            {
                if(projectileEntities.HasComponent(entity2))
                {

                    if (explosivEntities.HasComponent(entity2))
                    {
                        return;
                    }

                    
                    commandBuffer.AddComponent(entity2, new ExplosionTag());

                    if (playerHit.HasComponent(entity1))
                    {
                        return;
                    }

                    commandBuffer.AddComponent(entity1, new HitTag { damage = 50 });
                    Debug.Log("Une grenade a explosé");
                }

                
            }
        }

    }
}*/
