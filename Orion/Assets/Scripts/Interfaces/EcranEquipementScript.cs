using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcranEquipementScript : GestionMenu
{
    [Header("Références")]
    [SerializeField] private GestionMenu ecranChoixMissions;

    private new void Awake()
    {
        InitialisationControlInput();
    }

    //activer les boutons à l'activation du gameobject
    private void OnEnable()
    {
        ActiverBoutons();
    }

    private new void Start()
    {
        gestionInput.Menus.Cancel.performed += ctx => RetourChoixMissions();
        gestionInput.Menus.Enable();
    }

    //fonction de retour au menu de choix de mission
    private void RetourChoixMissions()
    {
        if(verrou == false)
        {
            DesactiverBoutons();
            ecranChoixMissions.gameObject.SetActive(true);
            gameObject.SetActive(false);
            ecranChoixMissions.StartCoroutine(ecranChoixMissions.Verrou());
        }
    }
}
