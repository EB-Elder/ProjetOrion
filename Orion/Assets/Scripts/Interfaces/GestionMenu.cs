using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

[Serializable]
public struct Ligne
{
    public List<Button> boutons;
}

public class GestionMenu : MonoBehaviour
{
    [SerializeField] List<Ligne> boutons;
    [SerializeField] private int ligneDepart;
    [SerializeField] private int colonneDepart;

    private Vector2 currentButton = new Vector2();
    private PlayerControls gestionInput;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        boutons[ligneDepart].boutons[colonneDepart].image.color = boutons[ligneDepart].boutons[colonneDepart].colors.highlightedColor;
        currentButton.x = ligneDepart;
        currentButton.y = colonneDepart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
