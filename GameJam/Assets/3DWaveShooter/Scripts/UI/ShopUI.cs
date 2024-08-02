using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Shop")]
    public GameObject shopScreen;

    [Header("Weapon Buttons")]
    public GameObject[] weaponButtons;
    public Color upgradeColor;
    public Color purchaseColor;

    [Header("Shop Buttons")]
    public Text refillHealthText;
    public Text refillAmmoText;
    public Text increaseSpeedText;
    public Image increaseSpeedProgressBar;

    [Header("Text")]
    public Text playerMoneyText;

    //Instance
    public static ShopUI inst;
    void Awake () { inst = this; }

    void Start ()
    {
        CreateWeaponButtons();
    }

    void Update ()
    {
        //If a wave isn't currently in progress.
        if(!GameManager.inst.waveInProgress)
        {
            //If player presses E, toggle the shop.
            if(Input.GetKeyDown(KeyCode.E))
            {
                ToggleShop(!shopScreen.activeInHierarchy);
            }
        }
    }

    //Creates the required amount of weapon buttons needed based on weapons in the WeaponManager.
    void CreateWeaponButtons ()
    {
        List<Weapon> weapons = WeaponManager.baseWeapons;

        //Loop through all the weapon buttons.
        for(int x = 0; x < weaponButtons.Length; ++x)
        {
            //If this button is going to be shown.
            if(x < weapons.Count)
            {
                weaponButtons[x].SetActive(true);

                //Set icons and text.
                weaponButtons[x].transform.Find("WeaponName").GetComponent<Text>().text = weapons[x].displayName.ToUpper();
                weaponButtons[x].transform.Find("WeaponIcon").GetComponent<Image>().sprite = weapons[x].uiIcon;

                Image upgradeProgressBar = weaponButtons[x].transform.Find("UpgradeButton/ProgressBar").GetComponent<Image>();

                //Is this the player's starting weapon?
                if(WeaponManager.inst.weaponScriptableObjects[x] == Player.inst.startingWeapon)
                {
                    int upgrades = weapons[x].upgrades.Length;

                    upgradeProgressBar.color = upgradeColor;

                    //Does this weapon have upgrades?
                    if(upgrades > 0)
                    {
                        upgradeProgressBar.fillAmount = 1.0f / (upgrades + 1);

                        weaponButtons[x].transform.Find("UpgradeButton/Upgrade").GetComponent<Text>().text = "UPGRADE";
                        weaponButtons[x].transform.Find("UpgradeButton/UpgradeCost").GetComponent<Text>().text = "$" + weapons[x].upgrades[0].cost.ToString();

                        int id = weapons[x].id;
                        weaponButtons[x].transform.Find("UpgradeButton").GetComponent<Button>().onClick.AddListener(delegate { OnUpgradeButton(id); });
                    }
                    //Does this weapon NOT have upgrades?
                    else
                    {
                        upgradeProgressBar.fillAmount = 1.0f;

                        weaponButtons[x].transform.Find("UpgradeButton/Upgrade").GetComponent<Text>().text = "UPGRADE";
                        weaponButtons[x].transform.Find("UpgradeButton/UpgradeCost").GetComponent<Text>().text = "MAX";
                    }
                }
                //If not do this...
                else
                {
                    upgradeProgressBar.fillAmount = 1.0f;

                    weaponButtons[x].transform.Find("UpgradeButton/ProgressBar").GetComponent<Image>().color = purchaseColor;
                    weaponButtons[x].transform.Find("UpgradeButton/Upgrade").GetComponent<Text>().text = "PURCHASE";
                    weaponButtons[x].transform.Find("UpgradeButton/UpgradeCost").GetComponent<Text>().text = "$" + weapons[x].purchaseCost.ToString();

                    int id = weapons[x].id;
                    weaponButtons[x].transform.Find("UpgradeButton").GetComponent<Button>().onClick.AddListener(delegate { OnPurchaseButton(id); });
                }
            }
            //Otherwise disable it.
            else
                weaponButtons[x].SetActive(false);
        }
    }

    //Opens or closes the shop.
    public void ToggleShop (bool open)
    {
        shopScreen.SetActive(open);
        Player.inst.canAttack = !open;
        Player.inst.canMove = !open;

        if(open)
        {
            UpdateWeaponButtons();
            UpdateShop();
        }
    }

    //Updates the weapon buttons.
    void UpdateWeaponButtons ()
    {
        List<Weapon> playerWeapons = Player.inst.weapons;
        List<Weapon> allWeapons = WeaponManager.baseWeapons;

        //Loop through all the weapon buttons.
        for(int x = 0; x < weaponButtons.Length; ++x)
        {
            //Make sure we're only taking care of buttons we have weapons for.
            if(x < allWeapons.Count)
            {
                Weapon curWeapon = allWeapons[x];

                //Does the player own this weapon?
                if(Player.inst.GetWeapon(curWeapon.id) != null)
                {
                    Image upgradeProgressBar = weaponButtons[x].transform.Find("UpgradeButton/ProgressBar").GetComponent<Image>();

                    int upgrades = curWeapon.upgrades.Length;

                    upgradeProgressBar.color = upgradeColor;

                    //Does this weapon have upgrades left?
                    if(upgrades > 0 && curWeapon.nextUpgradeIndex < upgrades)
                    {
                        float rate = 1.0f / (float)(upgrades + 1);
                        upgradeProgressBar.fillAmount = rate * (curWeapon.nextUpgradeIndex + 1);

                        weaponButtons[x].transform.Find("UpgradeButton/Upgrade").GetComponent<Text>().text = "UPGRADE";
                        weaponButtons[x].transform.Find("UpgradeButton/UpgradeCost").GetComponent<Text>().text = "$" + curWeapon.upgrades[curWeapon.nextUpgradeIndex].cost.ToString();
                    }
                    //Does this weapon NOT have upgrades left?
                    else
                    {
                        upgradeProgressBar.fillAmount = 1.0f;

                        weaponButtons[x].transform.Find("UpgradeButton/Upgrade").GetComponent<Text>().text = "UPGRADE";
                        weaponButtons[x].transform.Find("UpgradeButton/UpgradeCost").GetComponent<Text>().text = "MAX";
                    }
                }
            }
        }
    }

    //Updates various shop UI elements like player money.
    void UpdateShop ()
    {
        playerMoneyText.text = "$" + Player.inst.money;

        //Refill Health and Ammo
        refillHealthText.text = "$" + ShopData.inst.refillHealthCost;
        refillAmmoText.text = "$" + ShopData.inst.refillAmmoCost;

        //Move Speed Upgrade
        if(ShopData.inst.moveSpeedUpgrade.canUpgrade)
            increaseSpeedText.text = "$" + ShopData.inst.moveSpeedUpgrade.curPrice;
        else
            increaseSpeedText.text = "MAX";

        float rate = 1.0f / (float)(ShopData.inst.moveSpeedUpgrade.maxUpgrades);
        increaseSpeedProgressBar.fillAmount = rate * ShopData.inst.moveSpeedUpgrade.upgradesDone;
    }

    //Called when a weapon's "Purchase" button is pressed.
    public void OnPurchaseButton (int weaponId)
    {
        Weapon weapon = WeaponManager.GetWeapon(weaponId);

        //Does the player have enough money to purchase?
        if(Player.inst.money >= weapon.purchaseCost)
        {
            //If so, remove the money and give the weapon.
            Player.inst.RemoveMoney(weapon.purchaseCost);
            Player.inst.GiveWeapon(weapon);

            //Change listener.
            weaponButtons[weaponId].transform.Find("UpgradeButton").GetComponent<Button>().onClick.RemoveAllListeners();
            weaponButtons[weaponId].transform.Find("UpgradeButton").GetComponent<Button>().onClick.AddListener(delegate { OnUpgradeButton(weaponId); });

            //Update shop.
            UpdateWeaponButtons();
            UpdateShop();
        }
    }

    //Called when a weapon's "Upgrade" button is pressed.
    public void OnUpgradeButton (int weaponId)
    {
        Weapon weapon = Player.inst.GetWeapon(weaponId);

        //Does the player have enough money to upgrade?
        if(Player.inst.money >= weapon.upgrades[weapon.nextUpgradeIndex].cost)
        {
            //If so, remove the money and upgrade.
            Player.inst.RemoveMoney(weapon.upgrades[weapon.nextUpgradeIndex].cost);
            weapon.Upgrade();

            //Update shop.
            UpdateWeaponButtons();
            UpdateShop();
        }
    }

    //Called when the "Refill Health" button is pressed.
    public void OnRefillHealthButton ()
    {
        //Does the player have enough money to refill?
        if(Player.inst.money >= ShopData.inst.refillHealthCost)
        {
            //Is so, take the money and refill health.
            Player.inst.RemoveMoney(ShopData.inst.refillHealthCost);
            Player.inst.curHp = Player.inst.maxHp;

            //Update shop.
            UpdateWeaponButtons();
            UpdateShop();
        }
    }

    //Called when the "Refill Ammo" button is pressed.
    public void OnRefillAmmoButton ()
    {
        //Does the player have enough money to refill?
        if(Player.inst.money >= ShopData.inst.refillAmmoCost)
        {
            //Is so, take the money and refill ammo.
            Player.inst.RemoveMoney(ShopData.inst.refillAmmoCost);

            for(int x = 0; x < Player.inst.weapons.Count; ++x)
                Player.inst.RefillAmmo(Player.inst.weapons[x].id);

            //Update shop.
            UpdateWeaponButtons();
            UpdateShop();
        }
    }

    //Called when the "Increase Speed" button is pressed.
    public void OnUpgradeMoveSpeedButton ()
    {
        //Does the player have enough money to upgrade move speed?
        if(Player.inst.money >= ShopData.inst.moveSpeedUpgrade.curPrice && ShopData.inst.moveSpeedUpgrade.canUpgrade)
        {
            ShopStatUpgrade upgrade = ShopData.inst.moveSpeedUpgrade;

            Player.inst.RemoveMoney(upgrade.curPrice);

            Player.inst.moveSpeed *= upgrade.statIncreaseModifier;

            upgrade.curPrice = Mathf.CeilToInt((float)upgrade.curPrice * upgrade.priceIncreaseRate);

            upgrade.upgradesDone++;

            if(upgrade.upgradesDone == upgrade.maxUpgrades)
                upgrade.canUpgrade = false;

            //Update shop.
            UpdateWeaponButtons();
            UpdateShop();
        }
    }
}