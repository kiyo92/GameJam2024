using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains data about the player.
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Stats")]
    public PlayerState state;                       //Current state of the player.
    public int curHp;                               //Player's current health (CAN change when playing).
    public int maxHp;                               //Player's maximum health (CAN NOT change when playing).
    public float moveSpeed;                         //Player's move speed in units per second.
    public int money;                               //Player's current money.

    [Header("Weapon")]
    public WeaponScriptableObject startingWeapon;   //Weapon the player starts the game with.
    public Weapon curWeapon;                        //Player's currently equipped weapon.
    //public int maxWeapons;                          //Maximum number of weapons the player can hold.
    public List<Weapon> weapons = new List<Weapon>();//Player's inventory of weapons.
    private GameObject curWeaponObject;             //Player's current weapon game object visual.

    [Header("Bools")]
    public bool canMove;                            //Is the player able to move?
    public bool canAttack;                          //Is the player able to use their weapon/s?

    [Header("Components")]
    public GameObject weaponPos;                    //Position the player will hold their weapon at.
    public PlayerMovement movement;                 //Player's PlayerMovement component.
    public PlayerAttack attack;                     //Player's PlayerAttack component.
    public AudioSource audioSource;                 //Player's Audio Source component.
    public Animator anim;                           //Player's Animator component.
    public MeshSetter meshSetter;                   //Player's MeshSetter.cs component.
    
    [Header("Enemy")]
    public GameObject currentEnemy;

    //Instance
    public static Player inst;                      //We create an instance (singelton) of the player so that it can be accessed from anywhere.
    void Awake () { inst = this; }

    void Start ()
    {
        //Get missing components.
        if(!movement) movement = GetComponent<PlayerMovement>();
        if(!attack) attack = GetComponent<PlayerAttack>();
        if(!audioSource) audioSource = GetComponent<AudioSource>();
        if(!anim) anim = transform.Find("PlayerModel").GetComponent<Animator>();
        if(!meshSetter) meshSetter = GetComponent<MeshSetter>();

        //Make sure to give the player their starting weapon.
        GiveWeapon(WeaponManager.GetWeapon(startingWeapon));
    }

    void Update ()
    {
        CheckInputs();
    }

    //Checks for keyboard inputs.
    void CheckInputs ()
    {
        //Weapon change.
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
            TryChangeWeapon(1);
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
            TryChangeWeapon(-1);
    }

    //Called when the player takes damage. From enemies or any other source in the world.
    public void TakeDamage (int damage)
    {
        if(curHp - damage > 0)
            curHp -= damage;
        else
            Die();

        //Sound effect.
        AudioManager.inst.Play(audioSource, AudioManager.inst.playerImpactSFX[Random.Range(0, AudioManager.inst.playerImpactSFX.Length)]);

        //Update UI.
        GameUI.inst.UpdateHealthBar();

        //Cam Shake
        CameraEffects.inst.Shake(0.1f, 0.1f, 10.0f);

        //Visual color change.
        StartCoroutine(DamageVisualFlash());
    }

    //Called when the player's health reaches 0. Kills them. Game over.
    public void Die ()
    {
        curHp = 0;
        canMove = false;
        canAttack = false;
        state = PlayerState.Dead;
        anim.SetTrigger("Die");
        GameManager.inst.LoseGame();
    }

    //Adds to the player's current health.
    public void AddHealth (int amount)
    {
        curHp += amount;

        if(curHp > maxHp)
            curHp = maxHp;
    }

    //Called for the player to receive a new weapon.
    public void GiveWeapon (Weapon weapon)
    {
        //Can the player hold more weapons? If not, return.
        /*if(weapons.Count >= maxWeapons)
            return;*/

        weapons.Add(weapon);

        //Is this weapon not already linked to a player weapon visual?
        if(!weapon.onPlayerVisual)
        {
            //Create the new visual.
            weapon.onPlayerVisual = Instantiate(weapon.visualPrefab, weaponPos.transform.position + weapon.offsets.positionOffset, weaponPos.transform.rotation, weaponPos.transform);
            weapon.onPlayerVisual.transform.localEulerAngles += weapon.offsets.rotationOffset;
            weapon.onPlayerVisual.SetActive(false);
        }

        //Finally equip the weapon.
        EquipWeapon(weapon);
    }

    //Tries to equip a weapon in arsenal.
    //dir is the direction of change (1 = next, -1 = last).
    void TryChangeWeapon (int dir)
    {
        int nextIndex = weapons.IndexOf(curWeapon) + dir;

        if(nextIndex < 0)
            EquipWeapon(weapons[weapons.Count - 1]);
        else if(nextIndex >= weapons.Count)
            EquipWeapon(weapons[0]);
        else
            EquipWeapon(weapons[nextIndex]);

        GameUI.inst.UpdateEquippedWeaponIcons();
    }

    //Equips the requested weapon, changing values that are needed and visuals.
    public void EquipWeapon (Weapon weapon)
    {
        Weapon prevWeapon = curWeapon;
        curWeapon = weapon;

        //Disable previous weapon visual, and enable the new one.
        if(prevWeapon != null)
            if(prevWeapon.onPlayerVisual != null)
                prevWeapon.onPlayerVisual.SetActive(false);

        curWeapon.onPlayerVisual.SetActive(true);

        GameUI.inst.UpdateEquippedWeaponIcons();
    }

    //Drops a weapon on the ground.
    public void DropWeapon (Weapon weaponToDrop)
    {
        //Does the player have the weapon? If not, return.
        if(weapons.Find(x => x.displayName == weaponToDrop.displayName) == null)
            return;

        //Spawn the weapon pickup.
        GameObject droppedWeapon = Instantiate(weaponToDrop.droppedPickup, transform.position, Quaternion.identity, null);
        droppedWeapon.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        droppedWeapon.GetComponent<Pickup>().SetWeapon(weaponToDrop);

        //Remove the weapon from the player.
        weapons.Remove(weaponToDrop);

        GameUI.inst.UpdateEquippedWeaponIcons();
    }

    //Adds money to the player's total.
    public void AddMoney (int amount)
    {
        money += amount;
    }

    //Removes money from the player's total.
    public void RemoveMoney (int amount)
    {
        money -= amount;
    }

    //Returns a player's weapon based on the id.
    public Weapon GetWeapon (int weaponId)
    {
        return weapons.Find(x => x.id == weaponId);
    }

    //Gives an amount of ammo to a certain weapon.
    public void GiveAmmo (int weaponId, int ammoAmount)
    {
        Weapon weapon = GetWeapon(weaponId);

        if(weapon == null)
            return;

        weapon.curAmmo += ammoAmount;
    }

    //Refills the ammo of the requested ammo.
    public void RefillAmmo (int weaponId)
    {
        Weapon weapon = GetWeapon(weaponId);

        if(weapon == null)
            return;

        weapon.curAmmo = weapon.totalAmmo - weapon.clipSize;
        weapon.curAmmoInClip = weapon.clipSize;
    }

    //Visually flashes the player red when damaged.
    IEnumerator DamageVisualFlash ()
    {
        //Color defaultColor = mr.material.color;
        //mr.material.color = Color.red;

        yield return new WaitForEndOfFrame();

        //mr.material.color = defaultColor;
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}

public enum PlayerState
{
    Idle,
    Moving,
    Dead
}