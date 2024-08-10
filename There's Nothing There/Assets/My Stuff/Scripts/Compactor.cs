using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compactor : MonoBehaviour
{
    public bool crushing = false;
    public float crushSpeed;

    public float waitTime;
    private float waitTimer;

    private void Start()
    {
        waitTimer = waitTime;
    }

    void Update()
    {
        GameObject crusher = gameObject.transform.GetChild(0).gameObject;

        if (crushing == true && crusher.transform.localPosition.x < 22.4f)
        {
            crusher.transform.localPosition += new Vector3(crushSpeed, 0, 0) * Time.deltaTime;
            waitTimer = waitTime;
        }
        else if(crusher.transform.localPosition.x > -45f)
        {
            crushing = false;
            waitTimer -= Time.deltaTime;
            if(waitTimer <= 0)
            {
                crusher.transform.localPosition -= new Vector3(crushSpeed, 0, 0) * Time.deltaTime;
            }
        }
    }
}
