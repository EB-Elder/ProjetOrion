using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;
using UnityEngine;

public class HitBossCollisionSystem : JobComponentSystem
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
        HittingBossJob hittingBossJob = new HittingBossJob
        {

            playerEntity = GetComponentDataFromEntity<PlayerStatsData>(),
            bossStats = GetComponentDataFromEntity<BossStats>(),
            bossHit = GetComponentDataFromEntity<HitTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer()

        };



        return hittingBossJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

    }

    private struct HittingBossJob : ICollisionEventsJob

    {

        // Le composant qui sert à identifier le joueur
        public ComponentDataFromEntity<PlayerStatsData> playerEntity;

        // Le composant qui sert à marquer un boss touché
        public ComponentDataFromEntity<BossStats> bossStats;


        // Le composant qui sert à marquer un boss touché
        [ReadOnly] public ComponentDataFromEntity<HitTag> bossHit;


        public EntityCommandBuffer commandBuffer;

        public void Execute(CollisionEvent collisionEvent)
        {
            Debug.Log("on touche un truc");
            TestEntityCollider(collisionEvent.Entities.EntityA, collisionEvent.Entities.EntityB);
            TestEntityCollider(collisionEvent.Entities.EntityB, collisionEvent.Entities.EntityA);

        }


        // On test si c'est bien un joueur qui entre en collision avec le boss
        // Si oui, on marque le boss comme étant touché

        // Cette fonction est utilisée juste au dessus dans le corp du job : la fonction execute
        private void TestEntityCollider(Entity entity1, Entity entity2)
        {
            if (playerEntity.HasComponent(entity1))
            {
                if (bossStats.HasComponent(entity2))
                {

                    if (bossHit.HasComponent(entity2))
                    {
                        return;
                    }

                    commandBuffer.SetComponent(entity2, new DeadTag());


                }


            }
        }

    }
}
