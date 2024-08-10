using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsaneAngel : MonoBehaviour
{
    [Header("Components")]
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject player;
    public Camera playerCamera;
    public GameObject flashlight;
    private bool lightOn;
    public Transform[] rayPoints;

    [Header("Behavior Settings")]
    public bool canTele = true;
    public float effectTime;
    private float effectTimer;
    public bool goingInsane = true;
    private int randNumEffect;

    [Header("Effect Settings")]
    private bool effectInProgress = false;
    public float pushTime;
    private float pushTimer;
    public float pushStrength;
    public float pushSet;

    void Start()
    {
        //Gets components
        player = GameObject.Find("Player");
        playerCamera = player.gameObject.transform.GetChild(1).gameObject.GetComponent<Camera>();
        flashlight = GameObject.Find("PlayerFlashlight");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        //Adds all the ray edges for angel
        rayPoints = new Transform[gameObject.transform.GetChild(1).gameObject.transform.childCount];
        for (int i = 0; i < rayPoints.Length; i++)
        {
            rayPoints[i] = gameObject.transform.GetChild(1).gameObject.transform.GetChild(i);
        }

        //Zeros variables
        pushTimer = pushTime;
        effectTimer = effectTime;
    }

    void Update()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            Debug.DrawRay(rayPoints[i].transform.position, playerCamera.transform.position - rayPoints[i].transform.position, Color.red);
        }

        //Light on check for abstraction
        if (flashlight.GetComponent<LightFollow>().isOn)
        {
            lightOn = true;
        }
        else
        {
            lightOn = false;
        }

        for (int i = 0; i < rayPoints.Length; i++)
        {
            Debug.DrawRay(rayPoints[i].transform.position, playerCamera.transform.position - rayPoints[i].transform.position, Color.red);
        }

        //If not being viewed
        if (!lightOn || !isBeingViewed())
        {
            //Look at the player when moving
            var lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Mathf.Infinity);

            //Tells the agent to move again and sets location to move to
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        //When were looked at again
        else
        {
            //Tells the agent to stop
            agent.isStopped = true;
        }



        //Attacks, insanity, and effects
        if (goingInsane)
        {
            //Activates the light when in striking distance
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 3 && (!isBeingViewed()))
            {
                gameObject.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.GetChild(2).gameObject.SetActive(false);
            }

            //Countdown for effects
            if(isBeingViewed() && lightOn && !effectInProgress)
            {
                if (effectTimer > 0)
                {
                    effectTimer -= Time.deltaTime;
                }
                else
                {
                    randNumEffect = Random.Range(1, 3);
                    effectInProgress = true;
                    effectTimer = effectTime;
                }
            }
            else
            {
                effectTimer = effectTime;
            }

            //Effects
            if (randNumEffect == 1)
            {
                if(pushTimer > 0)
                {
                    pushTimer -= Time.deltaTime;
                    float lookAngle = Vector3.SignedAngle(gameObject.transform.position - playerCamera.transform.position, playerCamera.transform.forward, Vector3.up);
                    pushSet = pushStrength * lookAngle / Mathf.Abs(lookAngle);
                }
                else
                {
                    randNumEffect = 0;
                    pushTimer = pushTime;
                    effectInProgress = false;
                    pushSet = 0;
                }
            }
            else if (randNumEffect == 2)
            {
                flashlight.GetComponent<LightFollow>().isOn = false;
                flashlight.GetComponent<LightFollow>().start = false;
                flashlight.GetComponent<LightFollow>().thisLight.enabled = false;
                randNumEffect = 0;
                effectInProgress = false;
            }
            else if (randNumEffect == 3)
            {
                Debug.Log("3");
            }
        }
    }

    //If this angel is in the frustum or hit by a raycast
    bool isBeingViewed()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        RaycastHit hit;

        //Checks to see if were in the players camera
        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) > 4)
            {
                //Checks just if middle is in range
                if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0), playerCamera.transform.position - transform.position, out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "Player" || hit.collider.tag == "humanHand")
                    {
                        return true;
                    }
                }

                //Loops through edge points
                for (int i = 0; i < rayPoints.Length; i++)
                {
                    if (Physics.Raycast(rayPoints[i].transform.position, playerCamera.transform.position - rayPoints[i].transform.position, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.tag == "Player" || hit.collider.tag == "humanHand")
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return true;
            }
            

            return false;
        }

        return false;
    }
}
