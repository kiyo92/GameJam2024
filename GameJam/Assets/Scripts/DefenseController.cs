using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseController : MonoBehaviour
{

    public GameObject currentTarget;
    public bool targetLocked;
    public GameObject defenderPosition;

    public GameObject trailPrefab;

    float shootInterval = 1f;
    float currentShootInterval = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLocked && currentTarget != null)
        {
            if (currentShootInterval <= 0)
            {
                print("atirando");
                Weapon weapon = Player.inst.curWeapon;
                ProjectileScriptableObject proj = weapon.projectile;

                float angleOffset = proj.multipleProjectiles.projectileSpreadAngle / 2;

                ShootProjectile(-angleOffset);
                currentShootInterval = shootInterval;
            }
            else
            {
                currentShootInterval -= Time.deltaTime;
            }

        }
        else
        {
            targetLocked = false;
            currentShootInterval = 0;
        }
    }

    void ShootProjectile(float angle)
    {
        Weapon weapon = Player.inst.curWeapon;

        //Rotate the weapon pos so we can get the angle for the projectile to travel at.
        defenderPosition.transform.LookAt(currentTarget.transform, Vector3.left);

        //Instantiate the projectile prefab.
        GameObject proj = Pool.Spawn(weapon.projectile.projectilePrefab, defenderPosition.transform.position, defenderPosition.transform.rotation);

        //Set the projectile's velocity to make it move.
        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * weapon.projectile.speed * 2;

        //Get the projectile script and set the damage and other values.
        Projectile projScript = proj.GetComponent<Projectile>();

        //Create the trail object and get the LineRenderer component.
        GameObject trail = Pool.Spawn(trailPrefab, defenderPosition.transform.position, defenderPosition.transform.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        //Set the end position to be the projectile raycast length from the weapon.
        lr.SetPosition(0, defenderPosition.transform.position);
        lr.SetPosition(1, defenderPosition.transform.position + (defenderPosition.transform.forward * weapon.projectile.raycastLength));

        //Now we can rotate the weapon pos back to 0.
        defenderPosition.transform.localEulerAngles = new Vector3(defenderPosition.transform.localEulerAngles.x, 0, defenderPosition.transform.localEulerAngles.z);

        //Set the colour of the trail.
        lr.startColor = weapon.projectile.trail.trailColor;
        lr.endColor = weapon.projectile.trail.trailColor;

        //Then destroy the trail.
        Pool.Destroy(trail, 0.05f);

        projScript.SetValues(weapon.projectile);
    }

    void OnTriggerStay(Collider c) 
    {
        if (c.gameObject.tag == "Enemy" && currentTarget == null) {
            if (!targetLocked) {
                targetLocked = true;
                currentTarget = c.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider c) 
    {
        if (c.gameObject.tag == "Enemy" == c.gameObject)
        {
            targetLocked = false;
            currentTarget = null;
        }
    }
}
