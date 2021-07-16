using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowrdsCamera : MonoBehaviour
{
    public Transform Camera;

    Vector3 CamFollowVector;

    private void Start()
    {
        CamFollowVector = Camera.gameObject.GetComponent<CameraFollow>().Distance;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + CamFollowVector);
    }
}
