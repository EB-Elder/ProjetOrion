using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EcranTitreInterfaceScript : GestionMenu
{
    [SerializeField] private EcranChoixMissions MenuChoixMissions;

    private void OnEnable()
    {
        ActiverBoutons();
    }

    private void Start()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;

        boutons[0].boutons[0].onClick.AddListener(AfficherMenuChoixMissions);
        boutons[1].boutons[0].onClick.AddListener(Quitter);
        actif = true;
    }

    //clic sur le bouton jouer
    private void AfficherMenuChoixMissions()
    {
        if(boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true && verrou == false)
        {
            actif = false;
            DesactiverBoutons();
            MenuChoixMissions.gameObject.SetActive(true);
            gameObject.SetActive(false);
            MenuChoixMissions.StartCoroutine(MenuChoixMissions.Verrou());
            MenuChoixMissions.ChargerEquipement();
            MenuChoixMissions.actif = true;
        }
    }

    //quitter le jeu une fois
    private void Quitter()
    {
        if (boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true && verrou == false)
        {
            Application.Quit();
        }
    }
}
