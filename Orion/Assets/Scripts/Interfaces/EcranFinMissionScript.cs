using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EcranFinMissionScript : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Image background;
    [SerializeField] private InventaireManagerScript inventaire;
    [SerializeField] private InGameInterfaceScript ingameInterface;
    [SerializeField] private EcranChoixMissions ecranChoixMissions;
    [SerializeField] private EcranEquipementScript ecranEquipement;

    [Header("Références internes")]
    [SerializeField] private GameObject fenetreButin;
    [SerializeField] private Image backgroundRes;
    [SerializeField] private Image itemSprite;
    [SerializeField] private Text result;
    [SerializeField] private Text descItem;
    [SerializeField] private Text titreItem;
    [SerializeField] private Button boutonRetour;

    [Header("Composantes")]
    [SerializeField] private Sprite victoire;
    [SerializeField] private Sprite defaite;

    //afficher le résultat de la mission en fonction du paramètre
    public void AfficherResultat(bool res)
    {
        //en cas de victoire
        if(res)
        {
            backgroundRes.sprite = victoire;
            result.text = "VICTOIRE";
            result.color = Color.green;

            //accorder un loot au hasard
            int newItem = FindNewItem();

            if(newItem == -1)
            {
                //pas de nouveau item
                fenetreButin.SetActive(false);
            }
            else
            {
                inventaire.AjoutItem(newItem);
                fenetreButin.SetActive(true);

                //afficher les infos du nouvel item
                var listItemDispo = inventaire.GetListeEquipDispo();
                var item = inventaire.GetItemScriptFromEquipDispo(listItemDispo.Count - 1);
                titreItem.text = item.GetNom();
                descItem.text = item.GetDescription();
                itemSprite.sprite = item.GetIcone();
            }
            
            boutonRetour.gameObject.SetActive(true);
            boutonRetour.interactable = true;
            boutonRetour.image.color = boutonRetour.colors.highlightedColor;
        }
        else
        {
            //en cas de défaite
            backgroundRes.sprite = defaite;
            result.text = "DEFAITE";
            result.color = Color.red;
            fenetreButin.SetActive(false);
            boutonRetour.gameObject.SetActive(true);
            boutonRetour.interactable = true;
            boutonRetour.image.color = boutonRetour.colors.highlightedColor;
        }
    }

    //définir un index pour le nouvel item à ajouter à la liste des items dispos
    private int FindNewItem()
    {
        var listItemDispo = inventaire.GetListeEquipDispo();

        //si le joueur a tous les items, pas besoin de lui en donner un autre
        if(inventaire.GetNbAllItem() == listItemDispo.Count)
        {
            return -1;
        }
        else
        {
            List<int> potentialItems = new List<int>();

            //pour chaque indice d'item
            for(int i = 0; i < inventaire.GetNbAllItem(); i++)
            {
                bool itemRecup = false;

                //on vérifie si cet indice est dans la liste des items dispos
                for(int j = 0; j < listItemDispo.Count; j++)
                {
                    //si c'est le cas, on ne l'ajoute pas à la liste des items potentiels
                    if (listItemDispo[j] == i)
                    {
                        itemRecup = true;
                    }
                }

                if (!itemRecup)
                {
                    potentialItems.Add(i);
                }
            }

            //renvoie un int aléatoire dans la liste des items potentiels
            return potentialItems[(int)Random.Range(0, potentialItems.Count)];
        }
    }

    //fonction du bouton
    public void RetourChoixMenu()
    {
        ecranEquipement.SauvegardeBuildJoueur();
        background.gameObject.SetActive(true);
        ecranChoixMissions.gameObject.SetActive(true);
        gameObject.SetActive(false);
        ecranChoixMissions.StartCoroutine(ecranChoixMissions.Verrou());
        ecranChoixMissions.ChargerEquipement();
        ecranChoixMissions.actif = true;
    }

    private void Update()
    {
        //si le joueur appuie sur A
        if(Input.GetButtonDown("A"))
        {
            boutonRetour.onClick.Invoke();
        }
    }
}
