using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public List<WeaponScriptableObject> weaponScriptableObjects = new List<WeaponScriptableObject>();
    public static List<Weapon> baseWeapons = new List<Weapon>();

    //Instance
    public static WeaponManager inst;

    void Awake ()
    {
        inst = this;
        CreateBaseWeapons();
    }

    //Creates the base weapons from the 
    void CreateBaseWeapons ()
    {
        baseWeapons = new List<Weapon>();

        for(int x = 0; x < weaponScriptableObjects.Count; ++x)
        {
            baseWeapons.Add(new Weapon(weaponScriptableObjects[x], x));
        }
    }

    //Returns weapon class based on the weapon scriptable object sent.
    public static Weapon GetWeapon (WeaponScriptableObject weaponScriptableObject)
    {
        return baseWeapons[WeaponManager.inst.weaponScriptableObjects.IndexOf(weaponScriptableObject)];
    }

    public static Weapon GetWeapon (int weaponId)
    {
        return baseWeapons.Find(x => x.id == weaponId);
    }

    //Compares the 2 weapons to determine wether or not they're the same.
    public static bool WeaponsAreTheSame (Weapon weapon1, Weapon weapon2)
    {
        //Do they have the same id? If so, return true, otherwise false.
        if(weapon1.id == weapon2.id)
            return true;
        else
            return false;
    }
}
