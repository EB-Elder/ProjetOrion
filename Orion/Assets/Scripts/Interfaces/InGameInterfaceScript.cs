using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Physics;

public enum Competences
{
    NONE,
    FORCESHIELD,
}
public class InGameInterfaceScript : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private EcranFinMissionScript ecranFinMission;
    [SerializeField] private InventaireManagerScript inventaire;

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

        //paramétrage du boss
        boss = eManager.Instantiate(convert);
        eManager.SetName(boss, "Boss");
        Vector3 pos = new Vector3(0, 1.7f, 0);
        eManager.SetComponentData(boss, new BossStats { health = pvBoss });
        eManager.SetComponentData(boss, new Translation { Value = pos });
        bossBar.maxValue = pvBoss;

        //replacer le joueur et lui remettre ses PV
        EntityQuery query = eManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        var res = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

        if(res.Length != 0)
        {
            eManager.SetComponentData<PhysicsVelocity>(res[0], new PhysicsVelocity { Linear = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f), Angular = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f) });
            eManager.SetComponentData<Translation>(res[0], new Translation { Value = new Unity.Mathematics.float3(0, 1, -7)} );
            eManager.SetComponentData<Rotation>(res[0], new Rotation { Value = new Unity.Mathematics.float4(0, 0, 0, 1) });
            eManager.SetComponentData<PlayerStatsData>(res[0], new PlayerStatsData { Health = 100, hit = false, movementSpeed = 60, rotationSpeed = 5, stamina = 100 });
            joueur = res[0];
        }

        enMission = true;
        res.Dispose();

        //lancer les systèmes
        eManager.World.GetExistingSystem<PlayerMovementSystem>().Enabled = true;
        eManager.World.GetExistingSystem<GrenadeMovementSystem>().Enabled = true;
        eManager.World.GetExistingSystem<ActivateGlobalAttackSystem>().Enabled = true;
        eManager.World.GetExistingSystem<TriggerSystem>().Enabled = true;
        eManager.World.GetExistingSystem<HitBossCollisionSystem>().Enabled = true;
        eManager.World.GetExistingSystem<ExplosionSystem>().Enabled = true;

        //lancer les systèmes des compétences équipées
        LectureInventaire();
    }

    //activer les systèmes correspondants aux items équipés par le joueur
    private void LectureInventaire()
    {
        List<ItemScript> build = new List<ItemScript>();

        build.Add(inventaire.GetTete());
        build.Add(inventaire.GetBuste());
        build.Add(inventaire.GetMainGauche());
        build.Add(inventaire.GetMainDroite());
        build.Add(inventaire.GetBottes());

        //pour chaque item équipé, activer le système qui correspond à sa compétence s'il en a une
        foreach(var item in build)
        {
            if(item != null)
            {
                if (item.skill != Competences.NONE)
                {
                    switch (item.skill)
                    {
                        case Competences.FORCESHIELD:
                            {
                                eManager.World.GetExistingSystem<ForceShieldSystem>().Enabled = true;
                                break;
                            }
                    }
                }
            }
        }
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

    //update la barre de stamina du joueur
    public void UpdateStamina(float value)
    {
        stBar.value = value;
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

        //mise à jour de la stamina du joueur
        var currentPlayerSt = eManager.GetComponentData<PlayerStatsData>(joueur).stamina;

        if (currentPlayerSt != stBar.value)
        {
            UpdateStamina(currentPlayerSt);
        }

        //vérification de fin de partie
        if (hpBar.value <= 0)
        {
            //arreter les systèmes
            eManager.World.GetExistingSystem<ActivateGlobalAttackSystem>().Enabled = false;
            eManager.World.GetExistingSystem<PlayerMovementSystem>().Enabled = false;
            eManager.World.GetExistingSystem<GrenadeMovementSystem>().Enabled = false;
            eManager.World.GetExistingSystem<TriggerSystem>().Enabled = false;
            eManager.World.GetExistingSystem<HitBossCollisionSystem>().Enabled = false;
            eManager.World.GetExistingSystem<ExplosionSystem>().Enabled = false;
            eManager.World.GetExistingSystem<ForceShieldSystem>().Enabled = false;

            //détruire l'entite du boss
            eManager.DestroyEntity(boss);
            settings.BlobAssetStore.Dispose();

            //détruire les projectiles
            EntityQuery query = eManager.CreateEntityQuery(ComponentType.ReadOnly<GrenadeInfernaleTag>());
            var res = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

            foreach(var e in res)
            {
                eManager.DestroyEntity(e);
            }

            res.Dispose();

            //défaite du joueur
            enMission = false;
            ecranFinMission.gameObject.SetActive(true);
            ecranFinMission.AfficherResultat(false);
            gameObject.SetActive(false);
        }
        else
        {
            if(bossBar.value <= 0)
            {
                //arreter les systèmes
                eManager.World.GetExistingSystem<ActivateGlobalAttackSystem>().Enabled = false;
                eManager.World.GetExistingSystem<PlayerMovementSystem>().Enabled = false;
                eManager.World.GetExistingSystem<GrenadeMovementSystem>().Enabled = false;
                eManager.World.GetExistingSystem<TriggerSystem>().Enabled = false;
                eManager.World.GetExistingSystem<HitBossCollisionSystem>().Enabled = false;
                eManager.World.GetExistingSystem<ExplosionSystem>().Enabled = false;
                eManager.World.GetExistingSystem<ForceShieldSystem>().Enabled = false;

                //détruire l'entite du boss
                eManager.DestroyEntity(boss);
                settings.BlobAssetStore.Dispose();

                //détruire les projectiles
                EntityQuery query = eManager.CreateEntityQuery(ComponentType.ReadOnly<GrenadeInfernaleTag>());
                var res = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

                foreach (var e in res)
                {
                    eManager.DestroyEntity(e);
                }

                res.Dispose();

                //victoire du joueur
                enMission = false;
                ecranFinMission.gameObject.SetActive(true);
                ecranFinMission.AfficherResultat(true);
                gameObject.SetActive(false);
            }
        }
    }
}
