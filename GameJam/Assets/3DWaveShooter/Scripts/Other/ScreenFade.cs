using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;         //UI image that will be used as the fade.

    private Color targetColor;      //Color that the screen is trying to fade to.
    private float targetDuration;   //Duration of the fade.
    private Vector4 targetDurMod;   //Explained below...

    /*          
     *          Let me explain what "targetDurMod" is...
     * 
     *   We want to increase each color component (r, g, b, a) so that they
     *   reach their target at the same time. Increasing them at a set rate
     *   will make the color look weird or different as it is reaching the
     *   target color at different times.
     *   
     */ 

    //Instance
    public static ScreenFade inst;
    void Awake () { inst = this; }
    
    void Start ()
    {
        fadeImage.color = Color.black;
        Fade(Color.clear, 0.5f);
    }

    //Fades the screen to a color over time.
    public void Fade (Color color, float duration)
    {
        targetColor = color;
        targetDuration = duration;

        targetDurMod = new Vector4(
            GetTargetDurationModifier(color.r, fadeImage.color.r),
            GetTargetDurationModifier(color.g, fadeImage.color.g),
            GetTargetDurationModifier(color.b, fadeImage.color.b),
            GetTargetDurationModifier(color.a, fadeImage.color.a));
    }

    float GetTargetDurationModifier (float colourComponent, float fadeImageColorComponent)
    {
        return Mathf.Clamp(Mathf.Abs(colourComponent - fadeImageColorComponent) / targetDuration, 0.0f, 99.0f);
    }

    void Update ()
    {
        if(fadeImage.color != targetColor)
        {
            fadeImage.color = new Color(
                Mathf.MoveTowards(fadeImage.color.r, targetColor.r, targetDurMod.x * Time.deltaTime),
                Mathf.MoveTowards(fadeImage.color.g, targetColor.g, targetDurMod.y * Time.deltaTime),
                Mathf.MoveTowards(fadeImage.color.b, targetColor.b, targetDurMod.z * Time.deltaTime),
                Mathf.MoveTowards(fadeImage.color.a, targetColor.a, targetDurMod.w * Time.deltaTime));
        }
    }
}