using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private bool shakingCam;            //Are we currently shaking the camera?

    //Instance
    public static CameraEffects inst;
    void Awake () { inst = this; }

    //Camera shake.
    public void Shake (float dur, float amount, float intensity)
    {
        StartCoroutine(CamShake(dur, amount, intensity));
    }

    //Shakes the camera.
    IEnumerator CamShake (float dur, float amount, float intensity)
    {
        if(shakingCam)
            yield return null;

        shakingCam = true;

        Camera cam = Camera.main;
        Vector3 originalPos = cam.transform.localPosition;
        Vector3 targetPos = originalPos + (Random.insideUnitSphere * amount);

        float t = 0.0f;

        while(t < dur)
        {
            cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, targetPos, intensity * Time.deltaTime);

            if(Vector3.Distance(cam.transform.localPosition, targetPos) < 0.01f)
            {
                targetPos = originalPos + (Random.insideUnitSphere * amount);
            }

            t += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = originalPos;
        shakingCam = false;
    }
}