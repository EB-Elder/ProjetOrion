using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventaireManagerScript : MonoBehaviour
{
    [Header("Items actuellement équipés")]
    [SerializeField] private int itemTete;
    [SerializeField] private int itemBuste;
    [SerializeField] private int itemMainGauche;
    [SerializeField] private int itemMainDroite;
    [SerializeField] private int itemBottes;

    [Header("Liste de tous les equipements")]
    [SerializeField] private List<GameObject> listeAllEquip;

    [Header("Liste des indices des equipements disponibles")]
    [SerializeField] private List<int> listeEquipDispo = new List<int>();


    #region getteurs pour les items équipés
    public ItemScript GetTete()
    {
        return GetItemInList(itemTete);
    }

    public ItemScript GetBuste()
    {
        return GetItemInList(itemBuste);
    }

    public ItemScript GetMainGauche()
    {
        return GetItemInList(itemMainGauche);
    }

    public ItemScript GetMainDroite()
    {
        return GetItemInList(itemMainDroite);
    }

    public ItemScript GetBottes()
    {
        return GetItemInList(itemBottes);
    }
    #endregion

    #region getteur pour les indices des items équipés
    public int GetIndiceTete()
    {
        return itemTete;
    }

    public int GetIndiceBuste()
    {
        return itemBuste;
    }

    public int GetIndiceMainG()
    {
        return itemMainGauche;
    }

    public int GetIndiceMainD()
    {
        return itemMainDroite;
    }

    public int GetIndiceBottes()
    {
        return itemBottes;
    }
    #endregion

    #region setteurs pour équiper des item
    public void SetTete(int i)
    {
        itemTete = i;
        if(i >= 0) listeAllEquip[itemTete].GetComponent<ItemScript>().SetEquip(true);
    }

    public void SetBuste(int i)
    {
        itemBuste = i;
        if (i >= 0) listeAllEquip[itemBuste].GetComponent<ItemScript>().SetEquip(true);
    }

    public void SetMainG(int i)
    {
        itemMainGauche = i;
        if (i >= 0) listeAllEquip[itemMainGauche].GetComponent<ItemScript>().SetEquip(true);
    }

    public void SetMainD(int i)
    {
        itemMainDroite = i;
        if (i >= 0) listeAllEquip[itemMainDroite].GetComponent<ItemScript>().SetEquip(true);
    }

    public void SetBottes(int i)
    {
        itemBottes = i;
        if (i >= 0) listeAllEquip[itemBottes].GetComponent<ItemScript>().SetEquip(true);
    }
    #endregion

    public ItemScript GetItemInList(int i)
    {
        if (i < 0) return null;
        return listeAllEquip[i].GetComponent<ItemScript>();
    }

    public List<int> GetListeEquipDispo()
    {
        return listeEquipDispo;
    }

    public int GetNbAllItem()
    {
        return listeAllEquip.Count;
    }

    public int GetIndiceItem(int i)
    {
        return listeEquipDispo[i];
    }

    public void AjoutItem(int i)
    {
        listeEquipDispo.Add(i);
    }

    public ItemScript GetItemScriptFromEquipDispo(int i)
    {
        return GetItemInList(listeEquipDispo[i]);
    }

    public void ChargerListItemDispo(List<string> itemInfos)
    {
        listeEquipDispo.Clear();

        for(int i = 1; i < itemInfos.Count; i++)
        {
            if(itemInfos[i] != "")
            {
                listeEquipDispo.Add(int.Parse(itemInfos[i]));
            }
        }
    }
}
