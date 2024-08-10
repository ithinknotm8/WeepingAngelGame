using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemReset : MonoBehaviour
{
    public GameObject powerActivator;
    private int prevNumActi = 0;
    private bool touching = false;

    public Material currLights;
    public GameObject lights;
    void Start()
    {

    }

    void Update()
    {
        powerActivator.GetComponent<PowerActivator>();

        if (touching && Input.GetKey(KeyCode.E) && PowerActivator.numFixed > prevNumActi)
        {
            lights.SetActive(true);
            currLights.SetColor("_EmissionColor", Color.white);
            prevNumActi = PowerActivator.numFixed;
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
