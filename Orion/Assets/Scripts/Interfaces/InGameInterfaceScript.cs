using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine.UI;

[AddComponentMenu("DOTS Samples/SpawnFromMonoBehaviour/Spawner")]
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
        Vector3 pos = new Vector3(0, 1.7f, 0);
        eManager.SetComponentData(boss, new BossStats { health = pvBoss });
        eManager.SetComponentData(boss, new Translation { Value = pos });

        bossBar.maxValue = pvBoss;

        //instancier le joueur en Entity
        /*var convertP = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerGO, settings);
        eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        joueur = eManager.Instantiate(convertP);
        Vector3 posP = new Vector3(-2, 1.5f, 0);
        eManager.SetComponentData(joueur, new Translation { Value = posP });
        eManager.SetComponentData(joueur, new PlayerTag { });
        eManager.SetComponentData(joueur, new PlayerStatsData { Health = 200, movementSpeed = 100, hit = false });*/


        enMission = true;
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
        /*var currentPlayerHp = eManager.GetComponentData<PlayerStatsData>(joueur).Health;

        if (currentPlayerHp != hpBar.value)
        {
            UpdateHPJoueur(currentPlayerHp);
        }*/
    }
}
