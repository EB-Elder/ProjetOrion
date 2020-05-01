using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeEquip
{
    TETE,
    BUSTE,
    MAING,
    MAIND,
    BOTTES
}

public class ItemScript : MonoBehaviour
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private TypeEquip type;

    public Sprite GetIcone()
    {
        return itemSprite;
    }

    public void SetIcone(Sprite s)
    {
        itemSprite = s;
    }
}
