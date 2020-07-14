using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EcranEquipementScript : GestionMenu
{
    [Header("Références")]
    [SerializeField] private EcranChoixMissions ecranChoixMissions;
    [SerializeField] private InventaireManagerScript inventaire;
    [SerializeField] private GameObject content;
    [SerializeField] private Text textInfoItem;
    [SerializeField] private Text NomItem;

    [Header("Références Equipement courant")]
    [SerializeField] private Image iconeTete;
    [SerializeField] private Image iconeBuste;
    [SerializeField] private Image iconeMainG;
    [SerializeField] private Image iconeMainD;
    [SerializeField] private Image iconeBottes;

    [Header("Prefab pour la scroll View")]
    [SerializeField] private GameObject scrollItemPrefab;

    private List<ItemScript> equipementDispoInfos = new List<ItemScript>();
    private List<Button> listButtonEquipDispo = new List<Button>();
    private int currentItem;

    //activer les boutons à l'activation du gameobject
    private void OnEnable()
    {
        ActiverBoutons();
    }

    private void Start()
    {
        actif = true;
    }

    //supprimer les items de la scroll view
    public void PurgeScrollView()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject go = content.transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }

    //générer les items dans la scroll view
    public void GenererItems()
    {
        var listeEquip = inventaire.GetListeEquipDispo();
        equipementDispoInfos.Clear();

        foreach(int i in listeEquip)
        {
            GameObject g = Instantiate<GameObject>(scrollItemPrefab);
            Image itemImage = g.GetComponent<Image>();
            ItemScript itemInfo = inventaire.GetItemInList(i);
            equipementDispoInfos.Add(itemInfo);
            itemImage.sprite = itemInfo.GetIcone();
            g.transform.SetParent(content.transform);
            g.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            listButtonEquipDispo.Add(g.GetComponent<Button>());
        }

        ChargerBuildFromFile();
        currentItem = 0;

        //boucle pour attribuer la bonne couleurs aux équipements en fonction de s'ils sont équipés ou non
        for(int i = 0; i < listButtonEquipDispo.Count; i++)
        {
            if(i == 0)
            {
                listButtonEquipDispo[i].image.color = listButtonEquipDispo[i].colors.highlightedColor;
            }
            else
            {
                if (inventaire.GetItemScriptFromEquipDispo(i).GetEquip())
                {
                    listButtonEquipDispo[i].image.color = listButtonEquipDispo[i].colors.pressedColor;
                }
                else
                {
                    listButtonEquipDispo[i].image.color = listButtonEquipDispo[i].colors.normalColor;
                }
            }
        }
    }

    //charger les icones de l'equipement courant du joueur dans l'interface
    public void ChargerEquipementCourant()
    {
        if (inventaire.GetTete() != null)
        {
            iconeTete.sprite = inventaire.GetTete().GetIcone();
            iconeTete.gameObject.SetActive(true);
        }
        else iconeTete.gameObject.SetActive(false);
        if (inventaire.GetBuste() != null)
        {
            iconeBuste.sprite = inventaire.GetBuste().GetIcone();
            iconeBuste.gameObject.SetActive(true);
        }
        else iconeBuste.gameObject.SetActive(false);
        if (inventaire.GetMainGauche() != null)
        {
            iconeMainG.sprite = inventaire.GetMainGauche().GetIcone();
            iconeMainG.gameObject.SetActive(true);
        }
        else iconeMainG.gameObject.SetActive(false);
        if (inventaire.GetMainDroite() != null)
        {
            iconeMainD.sprite = inventaire.GetMainDroite().GetIcone();
            iconeMainD.gameObject.SetActive(true);
        }
        else iconeMainD.gameObject.SetActive(false);
        if (inventaire.GetBottes() != null)
        {
            iconeBottes.sprite = inventaire.GetBottes().GetIcone();
            iconeBottes.gameObject.SetActive(true);
        }
        else iconeBottes.gameObject.SetActive(false);
    }

    //clear les sprites quand on quitte l'écran d'équipement
    private void ClearEquipementCourant()
    {
        iconeTete.sprite = null;
        iconeTete.gameObject.SetActive(false);

        iconeBuste.sprite = null;
        iconeBuste.gameObject.SetActive(false);

        iconeMainG.sprite = null;
        iconeMainG.gameObject.SetActive(false);

        iconeMainD.sprite = null;
        iconeMainD.gameObject.SetActive(false);

        iconeBottes.sprite = null;
        iconeBottes.gameObject.SetActive(false);

        listButtonEquipDispo.Clear();
    }

    //fonction de mise à jour du texte d'info de l'item courant
    private void UpdateTextInfoItem()
    {
        textInfoItem.text = equipementDispoInfos[currentItem].GetDescription();
        NomItem.text = equipementDispoInfos[currentItem].GetNom();
    }

    //fonction de navigation dans la scrollview (true pour scroll à droite, false pour scroll à gauche
    private void ScrollItems(bool b)
    {
        if(b)
        {
            if(currentItem < listButtonEquipDispo.Count - 1)
            {
                if (inventaire.GetItemScriptFromEquipDispo(currentItem).GetEquip())
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.pressedColor;
                }
                else
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.normalColor;
                }

                currentItem++;
                listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.highlightedColor;

                UpdateTextInfoItem();
            }
            else
            {
                if (inventaire.GetItemScriptFromEquipDispo(currentItem).GetEquip())
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.pressedColor;
                }
                else
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.normalColor;
                }

                currentItem = 0;
                listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.highlightedColor;

                UpdateTextInfoItem();
            }
        }
        else
        {
            if(currentItem > 0)
            {
                if (inventaire.GetItemScriptFromEquipDispo(currentItem).GetEquip())
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.pressedColor;
                }
                else
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.normalColor;
                }

                currentItem--;
                listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.highlightedColor;

                UpdateTextInfoItem();
            }
            else
            {
                if (inventaire.GetItemScriptFromEquipDispo(currentItem).GetEquip())
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.pressedColor;
                }
                else
                {
                    listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.normalColor;
                }

                currentItem = listButtonEquipDispo.Count - 1;
                listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.highlightedColor;

                UpdateTextInfoItem();
            }
        }
    }

    //fonction pour équiper l'item courant
    private void EquipCurrentItem()
    {
        var listeIndicesItem = inventaire.GetListeEquipDispo();

        switch(equipementDispoInfos[currentItem].GetTypeItem())
        {
            case TypeEquip.BOTTES:
                inventaire.SetBottes(listeIndicesItem[currentItem]);
                break;

            case TypeEquip.TETE:
                inventaire.SetTete(listeIndicesItem[currentItem]);
                break;

            case TypeEquip.BUSTE:
                inventaire.SetBuste(listeIndicesItem[currentItem]);
                break;

            case TypeEquip.MAIND:
                inventaire.SetMainD(listeIndicesItem[currentItem]);
                break;

            case TypeEquip.MAING:
                inventaire.SetMainG(listeIndicesItem[currentItem]);
                break;

            case TypeEquip.AMBIDEXTRE:
                
                //vérifier si l'une des mains est libre et mettre l'item dedans sinon dans la main droite par défaut
                if(inventaire.GetMainDroite() == null)
                {
                    inventaire.SetMainD(listeIndicesItem[currentItem]);
                }
                else
                {
                    if(inventaire.GetMainGauche() == null)
                    {
                        //si l'item à equiper est déjà dans la main droite
                        if(listeIndicesItem[currentItem] == inventaire.GetIndiceMainD())
                        {
                            inventaire.SetMainG(listeIndicesItem[currentItem]);
                        }
                        else
                        {
                            inventaire.SetMainG(listeIndicesItem[currentItem]);
                        }
                    }
                    else
                    {
                        inventaire.SetMainD(listeIndicesItem[currentItem]);
                    }
                }

                break;
        }
    }

    //fonction pour déséquiper l'item courant si celui-ci est équipé
    private void DesequipCurrentItem()
    {
        //si l'item est équipé
        if(inventaire.GetItemScriptFromEquipDispo(currentItem).GetEquip())
        {
            switch (inventaire.GetItemScriptFromEquipDispo(currentItem).GetTypeItem())
            {
                case TypeEquip.BOTTES:
                    inventaire.SetBottes(-1);
                    inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    break;

                case TypeEquip.TETE:
                    inventaire.SetTete(-1);
                    inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    break;

                case TypeEquip.BUSTE:
                    inventaire.SetBuste(-1);
                    inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    break;

                case TypeEquip.MAIND:
                    inventaire.SetMainD(-1);
                    inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    break;

                case TypeEquip.MAING:
                    inventaire.SetMainG(-1);
                    inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    break;

                case TypeEquip.AMBIDEXTRE:

                    //si l'item équipé est dans la main gauche
                    if(inventaire.GetIndiceMainG() == inventaire.GetIndiceItem(currentItem))
                    {
                        //si l'item de la main gauche est le même que celui de la main droite, on ne l'indique pas comme déséquipé (car encore dans la main droite)
                        if(inventaire.GetIndiceItem(currentItem) == inventaire.GetIndiceMainD())
                        {
                            inventaire.SetMainG(-1);
                        }
                        else
                        {
                            inventaire.SetMainG(-1);
                            inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                        }
                    }
                    else
                    {
                        //sinon il est dans la main droite
                        inventaire.SetMainD(-1);
                        inventaire.GetItemScriptFromEquipDispo(currentItem).SetEquip(false);
                    }
                    break;
            }

            //mettre à jour l'interface avec le nouvel équipement
            ChargerEquipementCourant();
        }
    }

    //fonction de sauvegarde de l'équipement du joueur
    public void SauvegardeBuildJoueur()
    {
        //création du fichier de sauvegarde dans le dossier Asset
        var writer = File.CreateText(Application.dataPath.ToString() + "/saveBuildPlayer.txt");

        //écrire les indices des différents slots d'item
        writer.WriteLine("head:" + inventaire.GetIndiceTete());
        writer.WriteLine("chest:" + inventaire.GetIndiceBuste());
        writer.WriteLine("handG:" + inventaire.GetIndiceMainG());
        writer.WriteLine("handD:" + inventaire.GetIndiceMainD());
        writer.WriteLine("feets:" + inventaire.GetIndiceBottes());

        string itemsDispo = "dispo:";

        foreach (var i in inventaire.GetListeEquipDispo())
        {
            itemsDispo += i + ":";
        }

        itemsDispo.Substring(0, itemsDispo.Length - 1);
        writer.WriteLine(itemsDispo);

        writer.Close();
    }

    //fonction de lecture du fichier de sauvegarde de l'équipement
    public void ChargerBuildFromFile()
    {
        string path = Application.dataPath.ToString() + "/saveBuildPlayer.txt";

        using (StreamReader sr = new StreamReader(path))
        {
            string ligne;

            while ((ligne = sr.ReadLine()) != null)
            {
                List<string> itemInfo = ligne.Split(new char[] { ':' }).ToList();

                switch (itemInfo[0])
                {
                    case "head":
                        inventaire.SetTete(int.Parse(itemInfo[1]));
                        break;

                    case "chest":
                        inventaire.SetBuste(int.Parse(itemInfo[1]));
                        break;

                    case "handG":
                        inventaire.SetMainG(int.Parse(itemInfo[1]));
                        break;

                    case "handD":
                        inventaire.SetMainD(int.Parse(itemInfo[1]));
                        break;

                    case "feets":
                        inventaire.SetBottes(int.Parse(itemInfo[1]));
                        break;

                    case "dispo":
                        inventaire.ChargerListItemDispo(itemInfo);
                        break;
                }
            }
        }
    }

    protected new void Update()
    {
        if (verrou) return;

        //équiper l'item courant
        if (Input.GetButtonDown("A"))
        {
            EquipCurrentItem();
            var newColor = listButtonEquipDispo[currentItem].colors.normalColor;
            listButtonEquipDispo[currentItem].image.color = listButtonEquipDispo[currentItem].colors.pressedColor;
            ChargerEquipementCourant();
            SauvegardeBuildJoueur();
            StartCoroutine(Verrou());
        }

        //retour au choix de la mission
        if (Input.GetButtonDown("B"))
        {
            SauvegardeBuildJoueur();
            RetourChoixMissions();
        }

        //déséquiper l'item courant
        if(Input.GetButtonDown("X"))
        {
            DesequipCurrentItem();
            StartCoroutine(Verrou());
            SauvegardeBuildJoueur();
        }

        if (Input.GetAxis("LeftJoystickX") > 0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            ScrollItems(true);
            StartCoroutine(Verrou());
        }

        if (Input.GetAxis("LeftJoystickX") < -0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            ScrollItems(false);
            StartCoroutine(Verrou());
        }

        if (Input.GetAxis("LeftJoystickY") < -0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {

        }

        if (Input.GetAxis("LeftJoystickY") > 0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {

        }
    }

    //fonction de retour au menu de choix de mission
    private void RetourChoixMissions()
    {
        if(verrou == false && actif != false)
        {
            actif = false;
            DesactiverBoutons();
            PurgeScrollView();
            ClearEquipementCourant();
            ecranChoixMissions.gameObject.SetActive(true);
            ecranChoixMissions.ChargerEquipement();
            gameObject.SetActive(false);
            ecranChoixMissions.StartCoroutine(ecranChoixMissions.Verrou());
            ecranChoixMissions.actif = true;
        }
    }
}
