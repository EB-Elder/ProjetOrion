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
    [SerializeField] private int currentLigne;
    [SerializeField] private int currentColonne;

    private PlayerControls gestionInput;

    private void Awake()
    {
        gestionInput = new PlayerControls();

        gestionInput.Menus.MoveDown.performed += ctx => Descendre();
        gestionInput.Menus.MoveUp.performed += ctx => Monter();
        gestionInput.Menus.MoveLeft.performed += ctx => SlideGauche();
        gestionInput.Menus.MoveRight.performed += ctx => SlideDroite();
        gestionInput.Menus.Validate.performed += ctx => ValiderBouton();
    }

    // Start is called before the first frame update
    void Start()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
        gestionInput.Menus.Enable();
    }

    //valider le bouton
    private void ValiderBouton()
    {
        boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
    }

    //faire descendre le curseur dans le menu
    private void Descendre()
    {
        if(currentLigne < boutons.Count -1)
        {
            BackToNormalBouton();
            currentLigne++;
            HighlightCurrentBouton();
        }
    }

    //faire monter le curseur dans le menu
    private void Monter()
    {
        if(currentLigne > 0)
        {
            BackToNormalBouton();
            currentLigne--;
            HighlightCurrentBouton();
        }
    }

    //faire passer le curseur sur la gauche
    private void SlideGauche()
    {
        if(currentColonne > 0)
        {
            BackToNormalBouton();
            currentColonne--;
            HighlightCurrentBouton();
        }
    }

    //faire passer le curseur sur la droite
    private void SlideDroite()
    {
        if(currentColonne < boutons[currentLigne].boutons.Count - 1)
        {
            BackToNormalBouton();
            currentColonne++;
            HighlightCurrentBouton();
        }
    }

    //mettre le bouton courant en surbrillance
    private void HighlightCurrentBouton()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
    }

    //remettre un bouton à son état de base
    private void BackToNormalBouton()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.normalColor;
    }

    public void testclic()
    {
        Debug.Log("Clic !");
    }
}
