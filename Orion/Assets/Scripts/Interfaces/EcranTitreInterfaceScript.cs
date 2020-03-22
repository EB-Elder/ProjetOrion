using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EcranTitreInterfaceScript : GestionMenu
{
    [SerializeField] private GestionMenu MenuChoixMissions;

    private new void Awake()
    {
        InitialisationControlInput();
        gestionInput.Menus.Validate.performed += ctx => boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
    }

    private void OnEnable()
    {
        ActiverBoutons();
    }

    private new void Start()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
        gestionInput.Menus.Enable();

        boutons[0].boutons[0].onClick.AddListener(AfficherMenuChoixMissions);
        boutons[1].boutons[0].onClick.AddListener(Quitter);
    }

    //clic sur le bouton jouer
    private void AfficherMenuChoixMissions()
    {
        if(boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true && verrou == false)
        {
            DesactiverBoutons();
            MenuChoixMissions.gameObject.SetActive(true);
            gameObject.SetActive(false);
            MenuChoixMissions.StartCoroutine(MenuChoixMissions.Verrou());
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
