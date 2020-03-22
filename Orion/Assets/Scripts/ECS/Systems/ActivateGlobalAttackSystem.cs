using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class ActivateGlobalAttackSystem : ComponentSystem
{

    private float spawnTimer;
    private Random random;

    protected override void OnCreate()
    {
        random = new Random(56);
    }
    protected override void OnUpdate()
    {
        spawnTimer -= Time.DeltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = 5f;

            for(int i= 0; i < 50; i = i+1)
            {
                float3 position = new float3(random.NextFloat(-50f, 50f), 2.5f, random.NextFloat(-50f, 50f));
                //On accède à notre entity grace à la variable globale du PrefabEntityComponent
                Entity spawnedEntity = EntityManager.Instantiate(PrefabEntityComponent.prefabEntity);

                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = position });
                EntityManager.SetComponentData(spawnedEntity, new MovementData { startPosition = position, movementSpeed = 4, up = UnityEngine.KeyCode.Z, goingUp = true });
            }
            
            

        }

    }

}