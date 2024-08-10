using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAngel : MonoBehaviour
{
    [Header("Components")]
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject player;
    public Camera playerCamera;
    public GameObject flashlight;
    public Transform[] rayPoints;

    LayerMask ignoreLayers;

    void Start()
    {
        ignoreLayers = ~(1 << LayerMask.NameToLayer("Enemy")); // or any other layer you want to ignore
        //Gets components
        player = GameObject.Find("Player");
        playerCamera = player.gameObject.transform.GetChild(1).gameObject.GetComponent<Camera>();
        flashlight = GameObject.Find("PlayerFlashlight");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        //Adds all the ray edges for angel
        rayPoints = new Transform[gameObject.transform.GetChild(1).gameObject.transform.childCount];
        for(int i = 0; i < rayPoints.Length; i++)
        {
            rayPoints[i] = gameObject.transform.GetChild(1).gameObject.transform.GetChild(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            Debug.DrawRay(rayPoints[i].transform.position, playerCamera.transform.position - rayPoints[i].transform.position, Color.red);
        }

        //If not being viewed
        if (!flashlight.GetComponent<LightFollow>().isOn || !isBeingViewed())
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
    }

    //If this angel is in the frustum or hit by a raycast
    bool isBeingViewed()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        RaycastHit hit;

        //Checks to see if were in the players camera
        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            //Checks just if middle is in range
            if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1, 0), playerCamera.transform.position - transform.position, out hit, Mathf.Infinity, ignoreLayers))
            {
                if (hit.collider.tag == "Player" || hit.collider.tag == "humanHand")
                {
                    return true;
                }
            }

            //Loops through edge points
            for (int i = 0; i < rayPoints.Length; i++)
            {
                if (Physics.Raycast(rayPoints[i].transform.position, playerCamera.transform.position - rayPoints[i].transform.position, out hit, Mathf.Infinity, ignoreLayers))
                {
                    if (hit.collider.tag == "Player" || hit.collider.tag == "humanHand")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        return false;
    }
}
