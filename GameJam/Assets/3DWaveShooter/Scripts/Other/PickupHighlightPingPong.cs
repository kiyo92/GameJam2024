using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHighlightPingPong : MonoBehaviour
{
    private float targetY = -0.5f;

    void Update ()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.MoveTowards(transform.localPosition.y, targetY, Time.deltaTime * 0.5f), transform.localPosition.z);

        if(transform.localPosition.y == targetY)
        {
            if(targetY == 0.0f)
                targetY = -0.5f;
            else
                targetY = 0.0f;
        }
    }
}
