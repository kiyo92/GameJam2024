using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Values")]
    public int damage;                          //Damage the projectile deals upon impact.
    public float destroyTime;                   //Seconds after creation that it gets destroyed.
    public float knockback;                     //Amount to knock the enemy back by.

    public EffectScriptableObject[] effects;    //Effects applied to hit entities.

    private bool valuesSet;                     //Used for knowing if the projectile has its value set.

    void OnEnable ()
    {
        destroyTime = Time.time + 1;
    }

    void FixedUpdate ()
    {
        //Destroy the projectile after a certain amount of time.
        if(Time.time > destroyTime)
            Pool.Destroy(gameObject);
    }

    void OnTriggerEnter (Collider col)
    {
        //Did we hit an enemy?
        if(col.tag == "Enemy")
        {
            col.GetComponent<Enemy>().TakeDamage(damage, transform.position, -Player.inst.transform.forward);
            col.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position).normalized * knockback, ForceMode.Impulse);

            for(int i = 0; i < effects.Length; ++i)
            {
                new Effect(effects[i], col.gameObject);
            }

            Pool.Destroy(gameObject);
        }
        else if(col.tag == "Damageable")
        {
            col.GetComponent<Damageable>().TakeDamage(damage);

            for(int i = 0; i < effects.Length; ++i)
            {
                new Effect(effects[i], col.gameObject);
            }

            Pool.Destroy(gameObject);
        }
        else if(col.gameObject.layer == 10)
        {
            for(int i = 0; i < effects.Length; ++i)
            {
                new Effect(effects[i], col.gameObject);
            }

            Pool.Destroy(gameObject);
        }
    }

    //Sets the required values of the projectile to this script.
    public void SetValues (ProjectileScriptableObject data)
    {
        damage = data.damage;
        destroyTime = Time.time + data.destroyTime;
        knockback = Player.inst.curWeapon.enemyKnockback;
        effects = data.effectsToApply;
    }
}
