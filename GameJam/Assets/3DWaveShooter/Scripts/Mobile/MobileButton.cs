using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour
{
    public bool pressed;
    public bool held;

    public void PointerDown ()
    {
        transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);
        held = true;
        StartCoroutine(PressedDelay());
    }

    public void PointerUp ()
    {
        transform.localScale = Vector3.one;
        held = false;
    }

    IEnumerator PressedDelay ()
    {
        pressed = true;
        yield return new WaitForEndOfFrame();
        pressed = false;
    }
}
