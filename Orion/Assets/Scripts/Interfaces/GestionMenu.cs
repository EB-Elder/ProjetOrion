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

public abstract class GestionMenu : MonoBehaviour
{
    [SerializeField] protected List<Ligne> boutons;
    [SerializeField] protected int currentLigne;
    [SerializeField] protected int currentColonne;

    protected bool verrou = false;
    public bool actif;

    protected void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
        }

        if (Input.GetAxis("LeftJoystickX") > 0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            SlideDroite();
        }

        if (Input.GetAxis("LeftJoystickX") < -0.2f && Input.GetAxis("LeftJoystickX") != 0f)
        {
            SlideGauche();
        }

        if (Input.GetAxis("LeftJoystickY") < -0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {
            Monter();
        }

        if (Input.GetAxis("LeftJoystickY") > 0.2f && Input.GetAxis("LeftJoystickY") != 0f)
        {
            Descendre();
        }
    }

    //rendre les boutons du menu interactibles
    public void ActiverBoutons()
    {
        for (int i = 0; i < boutons.Count; i++)
        {
            for (int j = 0; j < boutons[i].boutons.Count; j++)
            {
                boutons[i].boutons[j].interactable = true;
                boutons[i].boutons[j].gameObject.SetActive(true);
            }
        }
    }

    //rendre les boutons du menu non interactibles
    public void DesactiverBoutons()
    {
        for (int i = 0; i < boutons.Count; i++)
        {
            for (int j = 0; j < boutons[i].boutons.Count; j++)
            {
                boutons[i].boutons[j].interactable = false;
                boutons[i].boutons[j].gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
    }

    //faire descendre le curseur dans le menu
    protected void Descendre()
    {
        if (boutons.Count == 0 || actif == false) return;
        if (currentLigne < boutons.Count -1)
        {
            BackToNormalBouton();
            currentLigne++;
            HighlightCurrentBouton();
        }
    }

    //faire monter le curseur dans le menu
    protected void Monter()
    {
        if (boutons.Count == 0 || actif == false) return;
        if (currentLigne > 0)
        {
            BackToNormalBouton();
            currentLigne--;
            HighlightCurrentBouton();
        }
    }

    //faire passer le curseur sur la gauche
    protected void SlideGauche()
    {
        if (boutons.Count == 0 || actif == false) return;
        if (currentColonne > 0 && verrou == false)
        {
            BackToNormalBouton();
            currentColonne--;
            HighlightCurrentBouton();
            StartCoroutine(Verrou());
        }
    }

    //faire passer le curseur sur la droite
    protected void SlideDroite()
    {
        if (boutons.Count == 0 || actif == false) return;
        if(currentColonne < boutons[currentLigne].boutons.Count - 1 && verrou == false)
        {
            BackToNormalBouton();
            currentColonne++;
            HighlightCurrentBouton();
            StartCoroutine(Verrou());
        }
    }

    //mettre le bouton courant en surbrillance
    protected void HighlightCurrentBouton()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.highlightedColor;
    }

    //remettre un bouton à son état de base
    protected void BackToNormalBouton()
    {
        boutons[currentLigne].boutons[currentColonne].image.color = boutons[currentLigne].boutons[currentColonne].colors.normalColor;
    }

    //coroutine pour éviter les superpositions d'input
    public IEnumerator Verrou()
    {
        verrou = true;
        yield return new WaitForSeconds(0.15f);
        verrou = false;
    }
}
