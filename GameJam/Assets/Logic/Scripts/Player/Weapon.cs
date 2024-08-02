using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container used to define a weapon (copied from WeaponScriptableObject).
/// </summary>
[System.Serializable]
public class Weapon
{
    //Type
    public int id;                                  //Unique identifier for the weapon.
    public string displayName;                      //Name displayed on UI elements.
    public WeaponType typeOfWeapon;                 //Type of weapon.

    //Values
    public int totalAmmo;                           //Total amount of ammo this gun has.
    public int clipSize;                            //Amount of ammo in a clip.
    public int curAmmo;                             //Current amount of ammo remaining.
    public int curAmmoInClip;                       //Current amount of ammo in the clip.
    public float reloadTime;                        //Time it takes to reload the weapon.
    public float shootFrequency;                    //Min time between shots.
    public float accuracy;                          //Projectile spread.
    public int burstAmount;                         //Amount of projectiles shot in a burst.
    public float playerKnockback;                   //Rigidbody force applied to the player.
    public float enemyKnockback;                    //Rigidbody force applied to the hit enemy.

    //Prefabs
    public ProjectileScriptableObject projectile;   //Projectile shot by the weapon.
    public AudioClip shootSFX;                      //Sound effect played when weapon is shot.
    public GameObject visualPrefab;                 //Weapon game object in the player's hands.
    public GameObject onPlayerVisual;               //The already created visual that the player has.
    public GameObject droppedPickup;                //Game object spawned when the weapon is dropped.
    public Sprite uiIcon;                           //Icon shown on screen.

    //Offsets
    public WeaponOffsets offsets;                   //Offsets to weapon object.

    //Upgrades / Purchase
    public int purchaseCost;                        //Cost to purchase weapon in the store.
    public WeaponUpgrade[] upgrades;                //Weapon upgrades.
    public int nextUpgradeIndex;                    //Index for upgrades for the weapon's next upgrade.

    public Weapon (WeaponScriptableObject w, int id)
    {
        this.id = id;

        displayName = w.displayName;
        typeOfWeapon = w.typeOfWeapon;

        totalAmmo = w.totalAmmo;
        clipSize = w.clipSize;
        curAmmo = totalAmmo - clipSize;
        curAmmoInClip = clipSize;

        reloadTime = w.reloadTime;
        shootFrequency = w.shootFrequency;
        accuracy = w.accuracy;
        burstAmount = w.burstAmount;
        playerKnockback = w.playerKnockback;
        enemyKnockback = w.enemyKnockback;

        projectile = w.projectile;
        shootSFX = w.shootSFX;
        visualPrefab = w.visual;
        droppedPickup = w.droppedPickup;
        uiIcon = w.uiIcon;

        offsets = w.offsets;

        purchaseCost = w.purchaseCost;
        upgrades = w.upgrades;
    }

    //Called when the weapon is needed to upgrade.
    //Changes stats based on the WeaponUpgrade given.
    public void Upgrade ()
    {
        //If we don't have a next upgrade, return.
        if(nextUpgradeIndex >= upgrades.Length)
            return;

        WeaponUpgrade upgrade = upgrades[nextUpgradeIndex];
        nextUpgradeIndex++;

        if(upgrade.accuracy != 0)
            accuracy = upgrade.accuracy;
        if(upgrade.burstAmount != 0)
            burstAmount = upgrade.burstAmount;
        if(upgrade.clipSize != 0)
            clipSize = upgrade.clipSize;
        if(upgrade.enemyKnockback != 0)
            enemyKnockback = upgrade.enemyKnockback;
        if(upgrade.playerKnockback != 0)
            playerKnockback = upgrade.playerKnockback;
        if(upgrade.reloadTime != 0)
            reloadTime = upgrade.reloadTime;
        if(upgrade.shootFrequency != 0)
            shootFrequency = upgrade.shootFrequency;
    }
}