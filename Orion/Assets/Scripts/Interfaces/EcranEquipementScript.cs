using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcranEquipementScript : GestionMenu
{
    [Header("Références")]
    [SerializeField] private GestionMenu ecranChoixMissions;

    //activer les boutons à l'activation du gameobject
    private void OnEnable()
    {
        ActiverBoutons();
    }

    private void Start()
    {
        actif = true;
    }

    protected new void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            boutons[currentLigne].boutons[currentColonne].onClick.Invoke();
        }

        if (Input.GetButtonDown("B"))
        {
            RetourChoixMissions();
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

    //fonction de retour au menu de choix de mission
    private void RetourChoixMissions()
    {
        if(verrou == false && actif != false)
        {
            actif = false;
            DesactiverBoutons();
            ecranChoixMissions.gameObject.SetActive(true);
            gameObject.SetActive(false);
            ecranChoixMissions.StartCoroutine(ecranChoixMissions.Verrou());
            ecranChoixMissions.actif = true;
        }
    }
}
