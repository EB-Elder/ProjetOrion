using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class ActivateGlobalAttack : MonoBehaviour
{
    public GameObject grenadeInfernalPrefab;
    public Transform bossPosition;


    public Entity grenadeInfernale;

    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private Unity.Mathematics.Random random;

    private void Awake()
    {
        random = new Unity.Mathematics.Random(56);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        grenadeInfernale = GameObjectConversionUtility.ConvertGameObjectHierarchy(grenadeInfernalPrefab, settings);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float3 position = new float3(bossPosition.position.x + random.NextFloat(-5f, 5f), bossPosition.position.y + random.NextFloat(-5f, 5f), bossPosition.position.z + random.NextFloat(-5f, 5f));
            Entity spawnedEntity = entityManager.Instantiate(PrefabEntityComponent.prefabEntity);

            entityManager.SetComponentData(spawnedEntity, new Translation { Value = position });
            entityManager.SetComponentData(spawnedEntity, new MovementData { startPosition = position, movementSpeed = 4, up = UnityEngine.KeyCode.Z, goingUp = true });

        }

    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}
