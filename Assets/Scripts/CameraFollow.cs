using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetToFollow;
    
    private void Update()
    {
        //transform.position = new Vector3(
        //    Mathf.Clamp(targetToFollow.position.x, -27.13f, 2.98f),
        //    Mathf.Clamp(targetToFollow.position.y, Camera.main.transform.position.y - 9, Camera.main.transform.position.y + 9),
        //    transform.position.z);

        transform.position = new Vector3(
            targetToFollow.position.x,
            targetToFollow.position.y, 
            transform.position.z
            );
    }

    public void SetTarget(Transform target)
    {
        targetToFollow = target;
    }
}
