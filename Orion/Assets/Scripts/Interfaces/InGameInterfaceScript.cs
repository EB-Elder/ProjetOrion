using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;


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

    //Lancement de la mission
    public void LancerMission(GameObject bossGO)
    {
        //instancier le boss en Entity
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        boss = GameObjectConversionUtility.ConvertGameObjectHierarchy(bossGO, settings);
        eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        boss = eManager.Instantiate(boss);
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
}
