using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Header("Impact")]
    public GameObject bloodImpact;
    public GameObject environmentImpact;

    [Header("Explosion")]
    public GameObject explosion;

    //Instance
    public static ParticleManager inst;
    void Awake () { inst = this; }
}
