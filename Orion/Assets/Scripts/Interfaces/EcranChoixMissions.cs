using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Mission
{
    public GameObject boss;
    public GameObject decor;
    public string titreMission;
    public string descriptifMission;
}

public class EcranChoixMissions : GestionMenu
{
    [Header("Références")]
    [SerializeField] private GestionMenu ecranTitre;
    [SerializeField] private GameObject interfaceJeu;
    [SerializeField] private GestionMenu ecranEquipement;
    [SerializeField] private GameObject content;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabMission;

    [Header("Liste des missions")]
    [SerializeField] private List<Mission> listeDesMissions;

    private bool missionsGenerees = false;

    private new void Awake()
    {
        InitialisationControlInput();
        gestionInput.Menus.Validate.performed += ctx => boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
    }

    private void OnEnable()
    {
        ActiverBoutons();
        ChargerMissions();
    }

    //charger les différentes missions dans la scroll view
    private void ChargerMissions()
    {
        if (missionsGenerees) return;

        foreach(var m in listeDesMissions)
        {
            GameObject g = Instantiate<GameObject>(prefabMission);
            ScrollMissionScript scrollInfo = g.GetComponent<ScrollMissionScript>();
            scrollInfo.setText(m.titreMission);
            g.transform.SetParent(content.transform);
        }

        missionsGenerees = true;
    }

    // Start is called before the first frame update
    private new void Start()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
        gestionInput.Menus.Enable();

        boutons[0].boutons[0].onClick.AddListener(RetourEcranTitre);
        boutons[0].boutons[1].onClick.AddListener(LancerMission);
        boutons[0].boutons[2].onClick.AddListener(AfficherMenuEquipement);
        actif = true;
    }

    //fonction pour revenir à l'écran titre
    private void RetourEcranTitre()
    {
        if (boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true && verrou == false)
        {
            actif = false;
            DesactiverBoutons();
            ecranTitre.gameObject.SetActive(true);
            gameObject.SetActive(false);
            ecranTitre.StartCoroutine(ecranTitre.Verrou());
            ecranTitre.actif = true;
        }
    }

    //lancer la mission
    private void LancerMission()
    {
        if (boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true && verrou == false)
        {
            DesactiverBoutons();
            actif = false;
            interfaceJeu.gameObject.SetActive(false);
        }
    }

    //afficher le menu d'équipement
    private void AfficherMenuEquipement()
    {
        if (boutons[currentLigne].boutons[currentColonne].gameObject.activeSelf == true)
        {
            actif = false;
            DesactiverBoutons();
            ecranEquipement.gameObject.SetActive(true);
            gameObject.SetActive(false);
            ecranEquipement.StartCoroutine(Verrou());
            ecranEquipement.actif = true;
        }
    }
}
