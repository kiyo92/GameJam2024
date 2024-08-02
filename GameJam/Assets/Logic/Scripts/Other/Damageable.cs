using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health;                      //Health remaining for this object.
    public bool explosive;                  //Will this explode upon death?
    public ExplosiveOptions explosiveData;  //Explosion data, if explosive.
    public GameObject destroyParticle;      //Particle that is spawned on death.
    public AudioClip destroySFX;            //Sound played when destroyed.

    public void TakeDamage (int damage)
    {
        if(health - damage > 0)
            health -= damage;
        else
            Die();
    }

    public void Die ()
    {
        health = 0;

        if(explosive)
        {
            Explosion.Explode(explosiveData, transform.position);
        }

        if(destroySFX)
            AudioManager.inst.PlayWithExternalAudioSource(destroySFX, transform.position);

        if(destroyParticle)
            Pool.Spawn(destroyParticle, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
