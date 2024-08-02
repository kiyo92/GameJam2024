using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the player's shooting and attacking with weapons from inputs.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public LayerMask raycastProjectileLayerMask;        //Layers that will be detected when shooting a raycast.
    public GameObject trailPrefab;                      //Prefab of trail to draw for raycast projectiles.

    private float lastWeaponAttackTime;                 //Last time that the weapon was shot.
    private bool burstFiring;                           //Are we currently burst firing?
    private int curBurstProjectilesShot;                //Num of bullets shot in current burst.

    void Update ()
    { 
        //Mobile controls for shooting.
        if(MobileControls.inst.enableMobileControls)
        {
            //Shoot button press.
            if(MobileControls.inst.shootButton.pressed)
                TryToUseWeapon(false);

            //Shoot button hold.
            else if(MobileControls.inst.shootButton.held)
                TryToUseWeapon(true);
        }
        //Keyboard / Mouse controls for shooting.
        else
        {
            //Mouse down press.
            if(Input.GetKeyDown(KeyCode.Mouse0) && Player.inst.canAttack)
            {
                //Are we hovering over a raycastable UI element?
                if(!EventSystem.current.IsPointerOverGameObject())
                    TryToUseWeapon(false);
            }
            //Mouse down hold.
            else if(Input.GetKey(KeyCode.Mouse0) && Player.inst.canAttack)
            {
                //Are we hovering over a raycastable UI element?
                if(!EventSystem.current.IsPointerOverGameObject())
                    TryToUseWeapon(true);
            }

            //Reload.
            if(Input.GetKeyDown(KeyCode.R) && Player.inst.canAttack)
            {
                if(Player.inst.curWeapon.curAmmoInClip < Player.inst.curWeapon.clipSize)
                    Reload();
            }
        }
    }

    //Called when attack input is pressed or held down.
    void TryToUseWeapon (bool inputHeldDown)
    {
        Weapon weapon = Player.inst.curWeapon;

        //Are we able to shoot a projectile?
        if(Time.time - lastWeaponAttackTime > weapon.shootFrequency)
        {
            lastWeaponAttackTime = Time.time;

            //Do we have enough ammo?
            if(weapon.curAmmoInClip > 0)
            {
                //Are we holding down the shoot button?
                if(inputHeldDown)
                {
                    //Are we holding down but can only single shot? Return.
                    if(weapon.typeOfWeapon == WeaponType.SingleShot)
                        return;
                    //Are we not able to burst anymore? Return.
                    else if(weapon.typeOfWeapon == WeaponType.Burst && !burstFiring)
                        return;
                }

                //Are we a burst fire weapon and just pressed down? Enable burst.
                if(weapon.typeOfWeapon == WeaponType.Burst && !inputHeldDown)
                {
                    burstFiring = true;
                    curBurstProjectilesShot = 0;
                }

                Shoot();
            }
            else
            {
                if(weapon.curAmmo > 0)
                    Reload();
                else
                {
                    //Play dry fire sound effect.
                    if(weapon.typeOfWeapon == WeaponType.SingleShot)
                    {
                        if(!inputHeldDown)
                            AudioManager.inst.Play(Player.inst.audioSource, AudioManager.inst.dryFire);
                    }
                    else
                        AudioManager.inst.Play(Player.inst.audioSource, AudioManager.inst.dryFire);
                }
            }
        }
    }

    //Called for the weapon to shoot.
    void Shoot ()
    {
        Weapon weapon = Player.inst.curWeapon;
        ProjectileScriptableObject proj = weapon.projectile;

        //Are we shooting a physical projectile?
        if(Player.inst.curWeapon.projectile.type == ProjectileType.Projectile)
        {
            //Are we shooting multiple projectiles?
            if(proj.shootMultipleProjectiles)
            {
                float angleOffset = proj.multipleProjectiles.projectileSpreadAngle / 2;
                float segment = Mathf.Abs(proj.multipleProjectiles.projectileSpreadAngle / (proj.multipleProjectiles.projectileCount - 1));

                for(int i = 0; i < proj.multipleProjectiles.projectileCount; ++i)
                {
                    if(i == 0)
                        ShootProjectile(-angleOffset);
                    else if(i == proj.multipleProjectiles.projectileCount - 1)
                        ShootProjectile(angleOffset);
                    else
                        ShootProjectile((i * segment) - angleOffset);
                    
                }

            }
            //No? Then just the 1 straight.
            else
                ShootProjectile(0);
        }
        //Are we shooting a raycast?
        else
        {
            //Are we shooting multiple raycasts?
            if(proj.shootMultipleProjectiles)
            {
                float angleOffset = proj.multipleProjectiles.projectileSpreadAngle / 2;
                float segment = Mathf.Abs(proj.multipleProjectiles.projectileSpreadAngle / (proj.multipleProjectiles.projectileCount - 1));

                for(int i = 0; i < proj.multipleProjectiles.projectileCount; ++i)
                {
                    if(i == 0)
                        ShootRaycast(-angleOffset);
                    else if(i == proj.multipleProjectiles.projectileCount - 1)
                        ShootRaycast(angleOffset);
                    else
                        ShootRaycast((i * segment) - angleOffset);

                }

            }
            //No? Then just the 1 straight.
            else
                ShootRaycast(0);
        }

        //Reduce ammo.
        weapon.curAmmoInClip--;

        //If we're burst firing, keep track of amount of bullets shot.
        if(burstFiring)
        {
            curBurstProjectilesShot++;

            //Shot all the bullets we can in a burst? Disable burst.
            if(curBurstProjectilesShot == Player.inst.curWeapon.burstAmount)
                burstFiring = false;
        }

        //If we have no ammo left in clip, reload.
        if(weapon.curAmmoInClip == 0 && weapon.curAmmo > 0)
        {
            Reload();
        }

        //Play sound.
        AudioManager.inst.Play(Player.inst.audioSource, Player.inst.curWeapon.shootSFX);
    }

    //Shoots a projectile.
    void ShootProjectile (float angle)
    {
        Weapon weapon = Player.inst.curWeapon;

        //Rotate the weapon pos so we can get the angle for the projectile to travel at.
        Player.inst.weaponPos.transform.localEulerAngles = new Vector3(Player.inst.weaponPos.transform.localEulerAngles.x, angle + Random.Range(-weapon.accuracy / 2, weapon.accuracy / 2), Player.inst.weaponPos.transform.localEulerAngles.z);

        //Instantiate the projectile prefab.
        GameObject proj = Pool.Spawn(weapon.projectile.projectilePrefab, Player.inst.weaponPos.transform.position, Player.inst.weaponPos.transform.rotation);

        //Now we can rotate the weapon pos back to 0.
        Player.inst.weaponPos.transform.localEulerAngles = new Vector3(Player.inst.weaponPos.transform.localEulerAngles.x, 0, Player.inst.weaponPos.transform.localEulerAngles.z);

        //Set the projectile's velocity to make it move.
        proj.GetComponent<Rigidbody>().velocity = proj.transform.forward * weapon.projectile.speed;

        //Get the projectile script and set the damage and other values.
        Projectile projScript = proj.GetComponent<Projectile>();

        projScript.SetValues(weapon.projectile);
    }

    //Shoots a raycast.
    void ShootRaycast (float angle)
    {
        Weapon weapon = Player.inst.curWeapon;

        //Rotate the weapon pos so we can get the angle for the projectile to travel at.
        Player.inst.weaponPos.transform.localEulerAngles = new Vector3(Player.inst.weaponPos.transform.localEulerAngles.x, angle + Random.Range(-weapon.accuracy / 2, weapon.accuracy / 2), Player.inst.weaponPos.transform.localEulerAngles.z);      

        Ray ray = new Ray(Player.inst.weaponPos.transform.position, Player.inst.weaponPos.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, weapon.projectile.raycastLength, raycastProjectileLayerMask))
        {
            if(hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(weapon.projectile.damage, hit.point, hit.normal);
                hit.collider.GetComponent<Rigidbody>().AddForce(-hit.normal * weapon.enemyKnockback, ForceMode.Impulse);

                if(weapon.projectile.applyEffects)
                {
                    for(int i = 0; i < weapon.projectile.effectsToApply.Length; ++i)
                    {
                        new Effect(weapon.projectile.effectsToApply[i], hit.collider.gameObject);
                    }
                }
            }
            else if(hit.collider.tag == "Damageable")
            {
                hit.collider.GetComponent<Damageable>().TakeDamage(weapon.projectile.damage);
            }

            //Impact particle.
            GameObject impactParticle = Pool.Spawn(ParticleManager.inst.environmentImpact, hit.point, Quaternion.identity);
            impactParticle.transform.LookAt(hit.point + hit.normal);
        }

        //If we draw a trail, then do it.
        if(weapon.projectile.drawTrail)
        {
            //Make some local variables of other things so we don't need to write it all out.
            GameObject weaponPos = Player.inst.weaponPos;

            //Create the trail object and get the LineRenderer component.
            GameObject trail = Pool.Spawn(trailPrefab, weaponPos.transform.position, weaponPos.transform.rotation);
            LineRenderer lr = trail.GetComponent<LineRenderer>();

            //Set the end position to be the projectile raycast length from the weapon.
            lr.SetPosition(0, weaponPos.transform.position);
            lr.SetPosition(1, weaponPos.transform.position + (weaponPos.transform.forward * (hit.collider ? hit.distance : weapon.projectile.raycastLength)));

            //Now we can rotate the weapon pos back to 0.
            Player.inst.weaponPos.transform.localEulerAngles = new Vector3(Player.inst.weaponPos.transform.localEulerAngles.x, 0, Player.inst.weaponPos.transform.localEulerAngles.z);

            //Set the colour of the trail.
            lr.startColor = weapon.projectile.trail.trailColor;
            lr.endColor = weapon.projectile.trail.trailColor;

            //Then destroy the trail.
            Pool.Destroy(trail, 0.05f);
        }
        else
        {
            //Now we can rotate the weapon pos back to 0.
            Player.inst.weaponPos.transform.localEulerAngles = new Vector3(Player.inst.weaponPos.transform.localEulerAngles.x, 0, Player.inst.weaponPos.transform.localEulerAngles.z);
        }
    }

    //Called when cur ammo reaches 0.
    void Reload ()
    {
        if(Player.inst.curWeapon.curAmmo == 0)
            return;

        Player.inst.canAttack = false;
        GameUI.inst.PlayReloadDialAnimation(Player.inst.curWeapon.reloadTime);
        Invoke("ReloadComplete", Player.inst.curWeapon.reloadTime);
    }
    
    //Called when the reload has completed.
    void ReloadComplete ()
    {
        Weapon weapon = Player.inst.curWeapon;

        Player.inst.canAttack = true;

        if(weapon.curAmmo - weapon.clipSize > 0)
        {
            weapon.curAmmo -= weapon.clipSize - weapon.curAmmoInClip;
            weapon.curAmmoInClip = weapon.clipSize; 
        }
        else
        {
            weapon.curAmmoInClip = weapon.curAmmo;
            weapon.curAmmo = 0;
        }
    }
}