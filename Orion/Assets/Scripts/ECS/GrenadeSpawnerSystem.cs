
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class GrenadeSpawnerSystem : ComponentSystem
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

            //On accède à notre entity grace à la variable globale du PrefabEntityComponent
            Entity spawnedEntity = EntityManager.Instantiate(PrefabEntityComponent.prefabEntity);

            EntityManager.SetComponentData(spawnedEntity, new Translation { Value = new float3(random.NextFloat(-5f, 5f), random.NextFloat(-5f, 5f), 0) });


            /*//On cherche dans toute les entity celles qui ont un component prefabEntityComponent
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) =>
            {
                
                Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);

                // Et ici on le place à une coordonnée random
                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = new float3(random.NextFloat(-5f, 5f), random.NextFloat(-5f, 5f), 0) });
            });*/
        }
        
    }
}
