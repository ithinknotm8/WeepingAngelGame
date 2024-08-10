using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    Transform cameraTrans;
    public int speed = 10;
    public Light thisLight;
    public bool isOn = false;
    float timer = .1f;
    public bool start = false;

    void Start()
    {
        thisLight = GetComponent<Light>();
        cameraTrans = GameObject.Find("PlayerCamera").gameObject.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !start)
        {
            isOn = !isOn;
            if (isOn == false)
            {
                thisLight.enabled = isOn;
            }
            else
            {
                start = true;
            }
        }

        if (start)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                thisLight.enabled = isOn;
                timer = .05f;
                start = false;
            }
        }


        cameraTrans = GameObject.Find("PlayerCamera").gameObject.transform;
        gameObject.transform.position = cameraTrans.transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, cameraTrans.rotation, Time.deltaTime * speed);
    }
}
