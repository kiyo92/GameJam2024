using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSetter : MonoBehaviour
{
    private MeshRenderer[] meshRenderers;

    public Material defaultMaterial;
    public Material accentMaterial;

    void Awake ()
    {
        //Get all mesh renderer components in children.
        meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
    }

    //Sets the mesh renderers to have their default enemy materials.
    public void SetDefaultMaterial ()
    {
        foreach(MeshRenderer mr in meshRenderers)
        {
            if(mr.gameObject.name.Contains("Torso"))
                mr.material = accentMaterial;
            else
                mr.material = defaultMaterial;
        }
    }
}
