using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    [Header("Player hand")]
    public GameObject hand;
    public HumanHand handScript;

    [Header("Compactor")]
    public GameObject compactorWall1;
    public GameObject compactorWall2;
    public int closeSpeed;
    
    void Update()
    {
        handScript = hand.GetComponent<HumanHand>();

        if(handScript.numOrbsCollected == 4)
        {
            if (compactorWall1.transform.localPosition.z < -52.7f)
            {
                compactorWall1.transform.position += new Vector3(0, 0, closeSpeed) * Time.deltaTime;
            }
            if (compactorWall2.transform.localPosition.z > 47.4f)
            {
                compactorWall2.transform.position -= new Vector3(0, 0, closeSpeed) * Time.deltaTime;
            }
        }
    }
}
