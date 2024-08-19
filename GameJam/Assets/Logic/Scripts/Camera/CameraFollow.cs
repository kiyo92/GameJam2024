using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [Header("Values")]
    public GameObject target;
    public float distance;
    public Vector3 rotation;
    public Vector3 offset;

    void Start ()
    {
        if(!target)
            target = Player.inst.gameObject;
    }

    void Update ()
    {

        if(target)
            FollowTarget();
    }


    void FollowTarget ()
    {
        //Set the camera's rotation.
        transform.rotation = Quaternion.Euler(rotation);

        //Set the camera's position.
        transform.position = (target.transform.position + offset) + (-transform.forward * distance);
    }

    public void SetCameraLevel(int level) {
        if (level == 2)
        {
            distance = 40;
        }
        else if (level == 3)
        {

        }
    }
}
