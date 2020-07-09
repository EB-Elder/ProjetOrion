using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Physics;

//[AddComponentMenu("DOTS Samples/SpawnFromMonoBehaviour/Spawner")]
public class InGameInterfaceScript : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject playerGO;

    [Header ("Stats du joueur")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider stBar;

    [Header ("Stats du boss")]
    [SerializeField] private Slider bossBar;

    private EntityManager eManager;
    private Entity joueur;
    private Entity boss;
    private bool enMission = false;
    private GameObjectConversionSettings settings;

    //Lancement de la mission
    public void LancerMission(GameObject bossGO, int pvBoss)
    {
        //instancier le boss en Entity
        settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
        var convert = GameObjectConversionUtility.ConvertGameObjectHierarchy(bossGO, settings);
        eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        boss = eManager.Instantiate(convert);
        eManager.SetName(boss, "Boss");
        Vector3 pos = new Vector3(0, 1.7f, 0);
        eManager.SetComponentData(boss, new BossStats { health = pvBoss });
        eManager.SetComponentData(boss, new Translation { Value = pos });

        bossBar.maxValue = pvBoss;

        EntityQuery query = eManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        var res = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

        if(res.Length != 0)
        {
            eManager.SetComponentData<PhysicsVelocity>(res[0], new PhysicsVelocity { Linear = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f), Angular = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f) });
            eManager.SetComponentData<Translation>(res[0], new Translation { Value = new Unity.Mathematics.float3(0, 1, -7)} );
            eManager.SetComponentData<PlayerStatsData>(res[0], new PlayerStatsData { Health = 100, hit = false, movementSpeed = 60, rotationSpeed = 5 });
            joueur = res[0];
        }

        enMission = true;
        res.Dispose();
    }

    private void OnDestroy()
    {
        settings.BlobAssetStore.Dispose();
    }

    //update la barre d'hp du joueur
    public void UpdateHPJoueur(int value)
    {
        hpBar.value = value;
    }

    //update la barre d'hp du boss
    public void UpdateHPBoss(int value)
    {
        bossBar.value = value;
    }

    private void Update()
    {
        if (!enMission) return;

        //mise à jour des PV du boss
        var currentBossHp = eManager.GetComponentData<BossStats>(boss).health;

        if(currentBossHp != bossBar.value)
        {
            UpdateHPBoss(currentBossHp);
        }

        //mise à jour des PV du joueur
        var currentPlayerHp = eManager.GetComponentData<PlayerStatsData>(joueur).Health;

        if (currentPlayerHp != hpBar.value)
        {
            UpdateHPJoueur(currentPlayerHp);
        }
    }
}
