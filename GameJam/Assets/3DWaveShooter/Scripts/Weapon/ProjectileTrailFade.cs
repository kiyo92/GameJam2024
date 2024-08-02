using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrailFade : MonoBehaviour
{
    public LineRenderer lr;         //Line Renderer component.

    void OnEnable ()
    {
        StartCoroutine(Fade());
        //StartCoroutine(TrailMove());
    }

    IEnumerator TrailMove ()
    {
        Vector3 endPos = lr.GetPosition(1);
        lr.SetPosition(1, lr.GetPosition(0));

        while(lr.GetPosition(1) != endPos)
        {
            lr.SetPosition(1, Vector3.MoveTowards(lr.GetPosition(1), endPos, 20 * Time.deltaTime));
            yield return null;
        }
    }

    //Fades the alpha value of the line renderer to 0.
    IEnumerator Fade ()
    {
        Color clear = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, 0.0f);
        float a = lr.startColor.a;

        while(lr.endColor.a > 0)
        {
            lr.startColor = new Color(clear.r, clear.g, clear.b, a);
            lr.endColor = new Color(clear.r, clear.g, clear.b, a);

            a -= Time.deltaTime * 20;

            yield return null;
        }
    }
}
