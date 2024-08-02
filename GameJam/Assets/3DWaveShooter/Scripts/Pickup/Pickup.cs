using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives the player something when they collide with the pickup. One time use.
/// </summary>
public class Pickup : MonoBehaviour
{
    public PickupType type;                         //Type of pickup.

    [Header("Health")]
    public int healthToGive;                        //Health given to the player upon pickup.

    [Header("Ammo")]
    public int ammoToGive;                          //How much ammo is given to the player?
    public bool spreadAmmoAcrossAllWeapons;         //Is the ammo spread out across all weapons?

    [Header("Weapon")]
    public WeaponScriptableObject baseWeapon;       //Base weapon scriptable object.
    public Weapon weaponToGive;                     //Weapon given upon pickup.

    private float creationTime;                     //Used to make sure the player doesn't instantly pick their weapon up again after dropping it.
    
    void OnEnable ()
    {
        creationTime = Time.time;
    }

    void Start ()
    {
        if(baseWeapon != null)
            weaponToGive = WeaponManager.GetWeapon(baseWeapon);
    }

    //Sets the weapon to give.
    //Called when the player drops a weapon.
    public void SetWeapon (Weapon weapon)
    {
        weaponToGive = weapon;
    }

    void OnTriggerEnter (Collider col)
    {
        if(Time.time - creationTime < 0.2f)
            return;

        if(col.tag == "Player")
        {
            if(type == PickupType.Health)
            {
                Player.inst.AddHealth(healthToGive);
                Destroy(gameObject);
            }
            else if(type == PickupType.Weapon)
            {
                Player.inst.GiveWeapon(weaponToGive);
                Destroy(gameObject);
            }
            else if(type == PickupType.Ammo)
            {
                if(spreadAmmoAcrossAllWeapons)
                {
                    for(int x = 0; x < Player.inst.weapons.Count; ++x)
                    {
                        Player.inst.GiveAmmo(Player.inst.weapons[x].id, Mathf.FloorToInt((float)ammoToGive / Player.inst.weapons.Count));
                    }
                }
                else
                    Player.inst.GiveAmmo(Player.inst.curWeapon.id, ammoToGive);

                Destroy(gameObject);
            }
        }
    }
}

public enum PickupType
{
    Health,
    Weapon,
    Ammo
}