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

    [Header("Liste des equipements")]
    [SerializeField] private List<GameObject> listeAllEquip;

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

    public ItemScript GetItemInList(int i)
    {
        if (i < 0) return null;
        return listeAllEquip[i].GetComponent<ItemScript>();
    }
}
