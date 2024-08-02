using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Follows a target object.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Values")]
    public GameObject target;           //GameObject that the camera will follow.
    public float distance;              //Distance between the object and camera.
    public Vector3 rotation;            //Camera rotation.
    public Vector3 offset;              //Offset from target object.

    void Start ()
    {
        if(!target)
            target = Player.inst.gameObject;
    }

    void Update ()
    {
        //If we have a target, follow them.
        if(target)
            FollowTarget();
    }

    //Follows the target object.
    void FollowTarget ()
    {
        //Set the camera's rotation.
        transform.rotation = Quaternion.Euler(rotation);

        //Set the camera's position.
        transform.position = (target.transform.position + offset) + (-transform.forward * distance);
    }
}
