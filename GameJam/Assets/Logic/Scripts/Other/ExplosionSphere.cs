using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSphere : MonoBehaviour
{
    public MeshRenderer mr;     //Mesh Renderer component.
    public Vector3 startScale;  //Starting scale.
    private bool stopAnim;

    void OnEnable ()
    {
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation ()
    {
        Color c = mr.material.color;
        transform.localScale = startScale;

        while(transform.localScale.x != startScale.x + 1)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, startScale + Vector3.one, Time.deltaTime / 5);
            mr.material.color = new Color(c.r, c.g, c.b, Mathf.MoveTowards(mr.material.color.a, 0.0f, Time.deltaTime / 3));

            yield return null;
        }

        mr.material.color = c;
        gameObject.SetActive(false);
        transform.localScale = startScale;
    }
}
