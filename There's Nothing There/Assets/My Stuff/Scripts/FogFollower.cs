using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFollower : MonoBehaviour
{
    public GameObject objectToFollow;
    public float followSpeed;

    void Update()
    {
        gameObject.transform.position = new Vector3(Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x, followSpeed), objectToFollow.transform.position.y, Mathf.Lerp(transform.position.z, objectToFollow.transform.position.z, followSpeed));
    }
}
