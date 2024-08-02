using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homebrew;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
        [Foldout("Type", true)]

    [Tooltip("Name of the weapon.")]
    public string displayName;

    [Tooltip("Weapon behaviour.")]
    public WeaponType typeOfWeapon;

    [Foldout("Values", true)]
        
    [Tooltip("Default amount of ammunition.")]
    public int totalAmmo;

    [Tooltip("Size of ammo clip (bullets you can shoot before needing to reload).")]
    public int clipSize;

    [Tooltip("Time it takes to reload the weapon.")]
    public float reloadTime = 1.5f;

    [Tooltip("Time between shots.")]
    public float shootFrequency = 0.2f;

        [Tooltip("Random angle offset.")]

    [Range(0.0f, 5.0f)]
    public float accuracy = 0.1f;

    [Tooltip("Bullets per burst.")]
    public int burstAmount;

    [Tooltip("Player knockback.")]
    public float playerKnockback;

    [Tooltip("Enemy knockback.")]
    public float enemyKnockback;

        [Foldout("Prefabs", true)]

    [Tooltip("Object shot by the weapon.")]
    public ProjectileScriptableObject projectile;

    [Tooltip("Sound clip played when the weapon is shot.")]
    public AudioClip shootSFX;

    [Tooltip("Weapon visual game object.")]
    public GameObject visual;

    [Tooltip("Dropped version of the weapon. Used as a pickup.")]
    public GameObject droppedPickup;

    [Tooltip("UI icon shown on screen (500 x 500 px PNG).")]
    public Sprite uiIcon;

        [Foldout("Offsets", true)]

    [Tooltip("Weapon positional and rotation offsets.")]
    public WeaponOffsets offsets;

        [Foldout("Upgrades / Purchase", true)]

    [Header("How much does it cost to buy this gun in the store?")]
    public int purchaseCost;

    [Tooltip("Upgrades that change this weapons stats. Player can buy them in the store.")]
    public WeaponUpgrade[] upgrades;
}

[System.Serializable]
public class WeaponOffsets
{
    [Tooltip("Position offset relative to the player's hand.")]
    public Vector3 positionOffset;

    [Tooltip("Rotation offset relative to the player's hand.")]
    public Vector3 rotationOffset;
}

[System.Serializable]
public class WeaponUpgrade
{
    [Header("Cost to Upgrade")]
    [Tooltip("Cost to progress to this upgrade.")]
    public int cost;

    [Header("Stat Change")]
    [Header("(Only changes is value isn't 0)")]
    [Space]

    [Tooltip("New reload time.")]
    public float reloadTime;

    [Tooltip("New shoot frequency.")]
    public float shootFrequency;

    [Tooltip("New accuracy.")]
    public float accuracy;

    [Tooltip("New burst amount.")]
    public int burstAmount;

    [Tooltip("New clip size.")]
    public int clipSize;

    [Tooltip("New player knockback.")]
    public float playerKnockback;

    [Tooltip("New enemy knockback.")]
    public float enemyKnockback;
}

public enum WeaponType
{
    [Tooltip("Shoots a single projectile when mouse down.")]
    SingleShot,

    [Tooltip("Shoots multiple projectiles while mouse down.")]
    Automatic,

    [Tooltip("Shoots a set number of bullets while mouse down.")]
    Burst
}