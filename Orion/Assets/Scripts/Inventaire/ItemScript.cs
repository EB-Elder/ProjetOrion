using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeEquip
{
    TETE,
    BUSTE,
    MAING,
    MAIND,
    BOTTES,
    AMBIDEXTRE
}

public class ItemScript : MonoBehaviour
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private TypeEquip type;
    [SerializeField] private string nom;
    [SerializeField] private string description;
    [SerializeField] public Competences skill;

    [SerializeField]
    private bool equip = false;
    private bool equipAmbi = false;

    private void Start()
    {
        equip = false;
    }

    public Sprite GetIcone()
    {
        return itemSprite;
    }

    public bool GetEquip()
    {
        return equip;
    }

    public bool GetEquipAmbi()
    {
        return equipAmbi;
    }

    public void SetEquip(bool b)
    {
        equip = b;
    }

    public void SetIcone(Sprite s)
    {
        itemSprite = s;
    }

    public string GetNom()
    {
        return nom;
    }

    public string GetDescription()
    {
        return description;
    }

    public TypeEquip GetTypeItem()
    {
        return type;
    }
}
