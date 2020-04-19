using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class ScrollMissionScript : MonoBehaviour
{
    [SerializeField] private Text titreMission;

    public void setText(string t)
    {
        titreMission.text = t;
    }
}
