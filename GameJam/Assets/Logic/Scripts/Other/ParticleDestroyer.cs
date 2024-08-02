using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    public ParticleSystem particle;

    void OnEnable ()
    {
        Pool.Destroy(gameObject, particle.startLifetime);
        particle.Play();
    }
}
