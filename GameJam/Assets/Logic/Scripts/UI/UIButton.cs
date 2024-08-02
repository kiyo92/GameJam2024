using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public static Vector3 upScale = new Vector3(1.05f, 1.05f, 1.0f);
    private bool cancelScaling;

    public void OnEnter ()
    {
        StartCoroutine(ScaleButton(upScale));

        //Audio
        AudioManager.inst.Play(AudioManager.inst.uiAudioSource, AudioManager.inst.buttonHover, false);
    }

    public void OnExit ()
    {
        StartCoroutine(ScaleButton(Vector3.one));
    }

    public void OnClickDown ()
    {
        StartCoroutine(ScaleButton(Vector3.one));

        //Audio
        AudioManager.inst.Play(AudioManager.inst.uiAudioSource, AudioManager.inst.buttonClick, false);
    }

    public void OnClickUp ()
    {
        StartCoroutine(ScaleButton(upScale));
    }

    IEnumerator ScaleButton (Vector3 targetScale)
    {
        cancelScaling = true;
        yield return new WaitForEndOfFrame();
        cancelScaling = false;

        while(transform.localScale != targetScale)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime);

            if(cancelScaling)
            {
                cancelScaling = false;
                yield break;
            }

            yield return null;
        }
    }
}
