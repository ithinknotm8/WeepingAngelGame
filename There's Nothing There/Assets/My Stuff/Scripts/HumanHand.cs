using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHand : MonoBehaviour
{
    //Orb settings
    public float orbPickUpTime;
    public int numOrbsCollected;
    private float orbTimer;

    //Item checks
    public bool hasKeyCard = false;

    //Gameobject holder list
    private GameObject orb;
    private GameObject doorCard;
    private GameObject compactButton;
    private GameObject keyCard;

    //Bools for touching
    private bool touchingOrb = false;
    private bool touchingDoorLock = false;
    private bool touchingCompact = false;
    private bool touchingRCard = false;

    void Start()
    {
        orbTimer = orbPickUpTime;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (touchingOrb)
            {
                orbTimer -= Time.deltaTime;
            }
        }
        else
        {
            orbTimer = orbPickUpTime;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            if (touchingDoorLock && hasKeyCard)
            {
                doorCard.gameObject.GetComponent<DoorCard>().open = !doorCard.gameObject.GetComponent<DoorCard>().open;
            }
            else if (touchingDoorLock)
            {
                Debug.Log("play sound here");
            }

            if (touchingCompact)
            {
                compactButton.gameObject.GetComponent<Compactor>().crushing = true;
            }

            if (touchingRCard)
            {
                keyCard.gameObject.SetActive(false);
                hasKeyCard = true;
            }
        }


        if (orbTimer <= 0)
        {
            orb.SetActive(false);
            orbTimer = orbPickUpTime;
            numOrbsCollected++;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("orb"))
        {
            orb = other.gameObject;
            touchingOrb = true;
        }

        if (other.gameObject.CompareTag("cardDoor"))
        {
            doorCard = other.gameObject;
            touchingDoorLock = true;
        }

        if (other.gameObject.CompareTag("compactButton"))
        {
            compactButton = other.gameObject;
            touchingCompact = true;
        }

        if (other.gameObject.CompareTag("keyCard"))
        {
            keyCard = other.gameObject;
            touchingRCard = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("orb"))
        {
            touchingOrb = false;
        }

        if (other.gameObject.CompareTag("cardDoor"))
        {
            touchingDoorLock = false;
        }

        if (other.gameObject.CompareTag("compactButton"))
        {
            touchingCompact = false;
        }

        if (other.gameObject.CompareTag("keyCard"))
        {
            touchingRCard = false;
        }
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("orb"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                orb = other.gameObject;
                orbTimer -= Time.deltaTime;
            }
            else
            {
                orbTimer = orbPickUpTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.gameObject.CompareTag("cardDoor"))
            {
                other.gameObject.GetComponent<DoorCard>().open = !other.gameObject.GetComponent<DoorCard>().open;
            }
        }
    }*/
}
