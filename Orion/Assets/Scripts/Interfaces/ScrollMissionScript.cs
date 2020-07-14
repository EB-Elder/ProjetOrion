using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class ScrollMissionScript : MonoBehaviour
{
    [SerializeField] private Text titreMission;
    [SerializeField] private Button bouton;
    [SerializeField] private Image icone;

    private string description;
    private Scrollbar scrollBar;

    public void setImage(Sprite i)
    {
        icone.sprite = i;
    }

    public void setDescription(string d)
    {
        description = d;
    }

    public void setText(string t)
    {
        titreMission.text = t;
    }

    public void Highlight()
    {
        bouton.image.color = bouton.colors.highlightedColor;
    }

    public void Darklight()
    {
        bouton.image.color = bouton.colors.normalColor;
    }

    public void setScrollBar(Scrollbar s)
    {
        scrollBar = s;
    }
}
