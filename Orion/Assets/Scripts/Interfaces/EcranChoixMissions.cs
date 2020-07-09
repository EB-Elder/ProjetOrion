using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public struct Mission
{
    public GameObject boss;
    public GameObject decor;
    public int pvBoss;
    public string titreMission;
    public string descriptifMission;
    public Sprite icone;
}

public class EcranChoixMissions : GestionMenu
{
    [Header("Références")]
    [SerializeField] private GestionMenu ecranTitre;
    [SerializeField] private GameObject interfaceJeu;
    [SerializeField] private EcranEquipementScript ecranEquipement;
    [SerializeField] private InGameInterfaceScript ingameInterface;
    [SerializeField] private GameObject content;
    [SerializeField] private Text descriptif;
    [SerializeField] private InventaireManagerScript inventaire;
    [SerializeField] private GameObject background;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabMission;

    [Header("Inventaire")]
    [SerializeField] private Image tete;
    [SerializeField] private Image buste;
    [SerializeField] private Image mainGauche;
    [SerializeField] private Image mainDroite;
    [SerializeField] private Image bottes;

    [Header("Liste des missions")]
    [SerializeField] private List<Mission> listeDesMissions;

    //--------attributs privés
    private bool missionsGenerees = false;
    private ScrollMissionScript currentMission;
    private List<ScrollMissionScript> listeScrollMissions = new List<ScrollMissionScript>();
    private int index = 0;
    //--------

    private void OnEnable()
    {
        ActiverBoutons();
    }

    //charger les différentes missions dans la scroll view
    private void ChargerMissions()
    {
        if (missionsGenerees) return;

        foreach(var m in listeDesMissions)
        {
            GameObject g = Instantiate<GameObject>(prefabMission);
            ScrollMissionScript scrollInfo = g.GetComponent<ScrollMissionScript>();
            listeScrollMissions.Add(scrollInfo);
            if (m.titreMission != null) scrollInfo.setText(m.titreMission);
            scrollInfo.setDescription(m.descriptifMission);
            if(m.icone != null) scrollInfo.setImage(m.icone);
            g.transform.SetParent(content.transform);
        }

        missionsGenerees = true;
    }

    //charger les infos de l'inventaire dans les éléments de l'interface
    public void ChargerEquipement()
    {
        ecranEquipement.ChargerBuildFromFile();

        if (inventaire.GetTete() != null)
        {
            tete.sprite = inventaire.GetTete().GetIcone();
            tete.gameObject.SetActive(true);
        }
        else
        {
            tete.gameObject.SetActive(false);
        }
        if (inventaire.GetBuste() != null)
        {
            buste.sprite = inventaire.GetBuste().GetIcone();
            buste.gameObject.SetActive(true);
        }
        else
        {
            buste.gameObject.SetActive(false);
        }
        if (inventaire.GetMainGauche() != null)
        {
            mainGauche.sprite = inventaire.GetMainGauche().GetIcone();
            mainGauche.gameObject.SetActive(true);
        }
        else
        {
            mainGauche.gameObject.SetActive(false);
        }
        if (inventaire.GetMainDroite() != null)
        {
            mainDroite.sprite = inventaire.GetMainDroite().GetIcone();
            mainDroite.gameObject.SetActive(true);
        }
        else
        {
            mainDroite.gameObject.SetActive(false);
        }
        if (inventaire.GetBottes() != null)
        {
            bottes.sprite = inventaire.GetBottes().GetIcone();
            bottes.gameObject.SetActive(true);
        }
        else
        {
            bottes.gameObject.SetActive(false);
        }
    }

    private void UpdateMission(int i)
    {
        if(verrou == false)
        {
            //assombrir l'ancien bouton
            listeScrollMissions[index].Darklight();

            //passer au nouveau bouton et le highlight
            index = Math.Abs(i % listeScrollMissions.Count);
            currentMission = listeScrollMissions[index];
            currentMission.Highlight();

            //mettre à jour le texte descriptif
            descriptif.text = listeDesMissions[index].descriptifMission;

            //lancer la coroutine du verrou
            StartCoroutine(Verrou());
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //charger la liste des missions
        ChargerMissions();

        //update des boutons
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;

        boutons[0].boutons[0].onClick.AddListener(RetourEcranTitre);
        boutons[0].boutons[1].onClick.AddListener(LancerMission);
        boutons[0].boutons[2].onClick.AddListener(AfficherMenuEquipement);
        actif = true;
    }

    protected new void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
        }

        if (Input.GetButtonDown("B"))
        {
            RetourEcranTitre();
        }

        if (Input.GetAxis("LeftJoystickX") > 0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            SlideDroite();
        }

        if (Input.GetAxis("LeftJoystickX") < -0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            SlideGauche();
        }

        //scroll la liste des missions vers le haut
        if (Input.GetAxis("LeftJoystickY") < -0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {
            UpdateMission(index - 1);
        }

        //scroll la liste des missions vers le bas
        if (Input.GetAxis("LeftJoystickY") > 0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {
            UpdateMission(index + 1);
        }
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
            ingameInterface.gameObject.SetActive(true);
            ingameInterface.LancerMission(listeDesMissions[index].boss, listeDesMissions[index].pvBoss);
            background.SetActive(false);
            gameObject.SetActive(false);
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
            ecranEquipement.GenererItems();
            ecranEquipement.ChargerEquipementCourant();
            gameObject.SetActive(false);
            ecranEquipement.StartCoroutine(Verrou());
            ecranEquipement.actif = true;
        }
    }
}
