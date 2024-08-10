using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCard : MonoBehaviour
{
    public bool open = false;
    public float openSpeed;

    void Update()
    {
        GameObject door = transform.GetChild(0).gameObject;
        if (open)
        {
            if (door.transform.localPosition.z > -10.25)
            {
                door.transform.localPosition -= new Vector3(0, 0, openSpeed) * Time.deltaTime;
            }
        }
        else
        {
            if (door.transform.localPosition.z < -5.4)
            {
                door.transform.localPosition += new Vector3(0, 0, openSpeed) * Time.deltaTime;
            }
        }
    }
}
