using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform ObjectToFollow;
    public Vector3 Distance;
    public float LerpSpeed = 1f;

    void Start()
    {
        AdjustCameraPosition();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        AdjustCameraPosition();
    }

    public void AdjustCameraPosition()
    {
        if(ObjectToFollow == null)
        {
            return;
        }

        Vector3 newPosition = ObjectToFollow.position - Distance;
        transform.position = new Vector3(newPosition.x, Mathf.Lerp(transform.position.y, newPosition.y, LerpSpeed * Time.deltaTime), newPosition.z);
        transform.LookAt(ObjectToFollow);
    }
}
