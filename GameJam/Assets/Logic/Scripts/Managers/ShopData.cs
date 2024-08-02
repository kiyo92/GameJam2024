using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData : MonoBehaviour
{
    [Header("Refills")]
    public int refillHealthCost;
    public int refillAmmoCost;

    [Header("Stat Upgrades")]
    public ShopStatUpgrade moveSpeedUpgrade;

    //Instance
    public static ShopData inst;

    void Awake ()
    {
        inst = this;

        moveSpeedUpgrade.curPrice = moveSpeedUpgrade.basePrice;
    }
}

[System.Serializable]
public class ShopStatUpgrade
{
    public bool canUpgrade = true;      //Can the player purchase this upgrade?

    public int basePrice;               //Starting price for the first upgrade purchase.
    public float priceIncreaseRate;     //Rate at which the price increases per purchase.
    public int maxUpgrades;             //Max number of upgrades being able to be purchased for this stat.

    public float statIncreaseModifier;  //Modifier applied to the stat once purchased.

    public int upgradesDone;            //Current number of upgrades done (changed in script).
    public int curPrice;                //Current price for the upgrade (changed in script).
}