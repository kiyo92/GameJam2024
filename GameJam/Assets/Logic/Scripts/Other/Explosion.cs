using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioClip explosionSFX;
    public GameObject audioSourcePrefab;

    //Instance
    public static Explosion inst;
    void Awake () { inst = this; }

    //Global explosion function used by every thing that explodes.
    public static void Explode (ExplosiveOptions data, Vector3 source)
    {
        Explosion.inst.StartCoroutine(Explosion.inst.ExplodeTimer(data, source));
    }

    public IEnumerator ExplodeTimer (ExplosiveOptions data, Vector3 source)
    {
        yield return new WaitForSeconds(0.05f);
        
        //Get all objects in the explosive range.
        RaycastHit[] hits = Physics.SphereCastAll(source, data.explosiveRange, Vector3.up);

        //Loop through them all.
        for(int i = 0; i < hits.Length; ++i)
        {
            //If it's an enemy, damage them.
            if(hits[i].collider.tag == "Enemy")
            {
                hits[i].collider.GetComponent<Enemy>().TakeDamage(data.explosiveDamage);
            }
            else if(hits[i].collider.tag == "Damageable")
            {
                hits[i].collider.GetComponent<Damageable>().TakeDamage(data.explosiveDamage);
            }

            //Get rigidbody component.
            Rigidbody rig = hits[i].collider.GetComponent<Rigidbody>();

            //Add explosive force to the rigidbody.
            if(rig)
            {
                rig.AddExplosionForce(data.explosiveForce, source + (Vector3.one * Random.Range(-1.0f, 1.0f)), data.explosiveRange, 0.1f, ForceMode.Impulse);
            }
        }

        //Effect
        GameObject obj = Pool.Spawn(ParticleManager.inst.explosion, source, Quaternion.identity);
        obj.GetComponent<ExplosionSphere>().startScale = Vector3.one * (data.explosiveRange * 2);
        Pool.Destroy(obj, 2.0f);

        //Cam Shake
        CameraEffects.inst.Shake(0.25f, 0.7f, 30.0f);

        //Audio
        GameObject audioSource = Instantiate(Explosion.inst.audioSourcePrefab, source, Quaternion.identity);
        AudioManager.inst.Play(audioSource.GetComponent<AudioSource>(), Explosion.inst.explosionSFX);
        Destroy(audioSource, 5.0f);
    }
}
