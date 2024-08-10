using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerActivator : MonoBehaviour
{
    public static int numFixed = 0;
    private bool alreadyOn = false;
    private bool touching = false;

    public Material currLights;
    public GameObject lights;
    
    void Start()
    {
        //currLights.SetColor("_EmissionColor", Color.white);
        //currLights.DisableKeyword("_EMISSION");
        //currLights.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if(touching && Input.GetKey(KeyCode.E) && !alreadyOn)
        {
            numFixed++;
            alreadyOn = true;

            //currLights.SetColor("_EmissionColor", Color.black);
            //lights.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("humanHand"))
        {
            touching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        touching = false;
    }
}
