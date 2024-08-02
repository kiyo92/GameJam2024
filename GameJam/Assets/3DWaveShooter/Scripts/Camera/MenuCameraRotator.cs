using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraRotator : MonoBehaviour
{
    public float rotateSpeed;
    
    void Update ()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
