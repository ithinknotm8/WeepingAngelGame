using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class WeepingAngle : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playLocation;

    public Camera viewCheck;
    public Camera playerCamera;
    Texture2D image;
    RenderTexture targ;

    float minTeleRange = 15;
    int numTrys = 0;
    bool teleMode = false;
    bool alreadyLookedAway = false;
    public bool canTele = true;
    public GameObject flashlight;

    public int xStartVal;
    public int yStartVal;
    public int xFinalVal;
    public int yFinalVal;




    void Start()
    {
        playLocation = GameObject.Find("Player").transform;
        viewCheck = GameObject.Find("EnemyScanner").GetComponent<Camera>();

        image = new Texture2D(viewCheck.pixelWidth, viewCheck.pixelHeight);
        targ = new RenderTexture(image.width, image.height, 16, RenderTextureFormat.ARGB32);
        targ.Create();
    }

    void Update()
    {
        viewCheck.targetTexture = targ;
        RenderTexture.active = viewCheck.targetTexture;
        image.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0);

        if (!flashlight.GetComponent<LightFollow>().isOn || !isBeingViewed(image))
        {
            var lookPos = playLocation.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Mathf.Infinity);
            if (!alreadyLookedAway && Random.Range(0,10) < 3 && canTele)
            {
                teleport();
            }
            alreadyLookedAway = true;

            agent.isStopped = false;
            agent.SetDestination(playLocation.transform.position);
        }
        else
        {
            alreadyLookedAway = false;
            agent.isStopped = true;
        }



        //Lights off stuff
        /*
        if (!flashlight.GetComponent<LightFollow>().isOn)
        {
            LightsOff = true;
        }
        else
        {
            LightsOff = false;
        }
        */
        //Teleport Stuff
        if (teleMode && (isBeingViewed() || Vector3.Distance(transform.position, playLocation.position) < minTeleRange) && numTrys < 100)
        {
            int randX = Random.Range(-20, 20);
            int randZ = Random.Range(-20, 20);
            Vector3 teleLoc = new Vector3(randX, transform.position.y, randZ);
            transform.position = teleLoc;
            numTrys++;
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            rend.enabled = true;
            teleMode = false;
        }
    }

    //Starts the teleport phase
    void teleport()
    {
        numTrys = 0;
        int randX = Random.Range(-20, 20);
        int randZ = Random.Range(-20, 20);
        Vector3 teleLoc = new Vector3(randX, transform.position.y, randZ);
        transform.position = teleLoc;

        agent.isStopped = true;
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = false;
        teleMode = true;
    }

    //If this angel is in the frustum only, less accurate and won't move behind walls
    bool isBeingViewed()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(viewCheck);

        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds) && Vector3.Distance(transform.position, playLocation.position) > 2.2)
        {
            return true;
        }
        return false;
    }

    //Checks in frustum AND pixels
    bool isBeingViewed(Texture2D camTexture)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(viewCheck);

        if (GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds) && Vector3.Distance(transform.position, playLocation.position) > 2.2)
        {
            Vector3 screenPos = viewCheck.WorldToScreenPoint(transform.position);

            int xCurrVal = (int) Mathf.Round(screenPos.x - 100);
            int yCurrVal = (int) Mathf.Round(screenPos.y - 100);
            xFinalVal = (int)Mathf.Round(screenPos.x + 100);
            yFinalVal = (int)Mathf.Round(screenPos.y + 100);

            xStartVal = xCurrVal;
            yStartVal = yCurrVal;

            /*
            while (yCurrVal < yFinalVal)
            {
                while (xCurrVal < xFinalVal)
                {
                    if (camTexture.GetPixel(xCurrVal, yCurrVal) == new Color(1, 0, 1, 1))
                    {
                        return true;
                    }
                    xCurrVal += 5; //3
                }
                yCurrVal += 5; //5
                xCurrVal = 0;
            }
            return false; //false
            */

            // RGBA32 texture format data layout exactly matches Color32 struct
            var data = camTexture.GetPixelData<Color32>(0);

            // fill texture data with a simple pattern
            Color32 testColor = new Color(1, 0, 1, 1);
            int index = 0;
            for (int y = 0; y < camTexture.height - 10; y += 10)
            {
                for (int x = 0; x < camTexture.width - 10; x += 10)
                {
                    index = camTexture.width * y + x;

                    if (data[index].r == testColor.r && data[index].g == testColor.g && data[index].b == testColor.b)
                    {
                        return true;
                    }

                    //index++;
                }
            }
        }
        else if(GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            return true;
        }
        return false;
    }
}