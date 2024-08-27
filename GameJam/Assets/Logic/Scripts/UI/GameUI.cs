using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Health")]
    public Slider healthBarSlider;

    public TextMeshProUGUI playerMoneyText;
    public GameObject openShopButton;

    [Header("Equipped Weapons")]
    public Image[] equippedWeaponIcons;

    [Header("Ammo")]
    public Text ammoInClipText;
    public Text totalAmmoText;

    [Header("Reload")]
    public Image reloadDial;
    private bool reloading;

    [Header("Wave")]
    public Text waveText;
    public Text waveCountdownText;
    public GameObject nextWaveButton;

    [Header("End Game")]
    public Text endGameText;

    //Instance
    public static GameUI inst;
    void Awake () { inst = this; }

    void Update ()
    {
        if(Player.inst.curWeapon != null)
        {
            ammoInClipText.text = Player.inst.curWeapon.curAmmoInClip.ToString();
            totalAmmoText.text = "/" + Player.inst.curWeapon.curAmmo.ToString();
        }
    }

    void FixedUpdate ()
    {
        UpdateHealthBar();
        playerMoneyText.text = "$" + Player.inst.money;
    }

    void LateUpdate ()
    {
        //If we're reloading, make the reload dial follow the player's aim.
        if(reloading)
            reloadDial.rectTransform.position = Camera.main.WorldToScreenPoint(Player.inst.weaponPos.transform.position + (Player.inst.transform.forward * 1.5f));
    }

    //called when the player reloads their weapon.
    public void PlayReloadDialAnimation (float reloadTime)
    {
        StartCoroutine(PlayReloadDialAnim(reloadTime));
    }

    //Fills the reload dial for the duration of the reload speed.
    IEnumerator PlayReloadDialAnim (float reloadSpeed)
    {
        reloadDial.gameObject.SetActive(true);
        reloading = true;
        reloadDial.fillAmount = 1.0f;

        float multiplier = 1.0f / reloadSpeed;

        while(reloadDial.fillAmount != 0.0f)
        {
            reloadDial.fillAmount = Mathf.MoveTowards(reloadDial.fillAmount, 0.0f, multiplier * Time.deltaTime);
            yield return null;
        }

        reloadDial.gameObject.SetActive(false);
        reloading = false;
    }

    //Updates the health bar.
    public void UpdateHealthBar ()
    {
        float rate = 1.0f / Player.inst.maxHp;
        healthBarSlider.value = rate * Player.inst.curHp;

        //if(!flashingHealthBar)
        //   StartCoroutine(HealthBarFlash());
    }

    //Updates the visuals on the equipped weapon icons.
    public void UpdateEquippedWeaponIcons ()
    {
        for(int x = 0; x < equippedWeaponIcons.Length; ++x)
        {
            //Do we have a weapon to fill out?
            if(x < Player.inst.weapons.Count)
            {
                equippedWeaponIcons[x].gameObject.SetActive(true);

                equippedWeaponIcons[x].sprite = Player.inst.weapons[x].uiIcon;
                
                //Is this the equipped weapon?
                if(Player.inst.curWeapon.id == Player.inst.weapons[x].id)
                {
                    equippedWeaponIcons[x].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                else
                {
                    equippedWeaponIcons[x].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }
            }
            else
                equippedWeaponIcons[x].gameObject.SetActive(false);
        }
    }

    //Sets the wave text to be the current wave number.
    public void UpdateWaveText ()
    {
        waveText.text = EnemySpawner.inst.nextWaveIndex.ToString();
    }

    //Sets the wave countdown text to count down from the requested number.
    public IEnumerator SetWaveCountdownText (int countdownFrom)
    {
        waveCountdownText.gameObject.SetActive(true);
        waveText.text = GameManager.inst.curWave.ToString();
        int defaultFontSize = waveCountdownText.fontSize;

        int curNum = countdownFrom;

        while(curNum != 0)
        {
            waveCountdownText.text = curNum.ToString();
            curNum--;
            waveCountdownText.fontSize += 20;
            yield return new WaitForSeconds(1.0f);
        }

        waveCountdownText.fontSize = defaultFontSize;
        waveCountdownText.gameObject.SetActive(false);
    }

    //Called when the "Next Wave" button is pressed.
    public void OnNextWaveButton ()
    {
        nextWaveButton.SetActive(false);
        openShopButton.SetActive(false);
        GameManager.inst.SetNextWave();
    }

    //Sets the end game text, as either a victory or defeat.
    public void SetEndGameText (bool win)
    {
        endGameText.gameObject.SetActive(true);

        if(win)
            endGameText.text = "Victory!";
        else
            endGameText.text = "Game Over!";
    }

    //Called when a weapon icon has been pressed.
    public void OnEquipWeapon (int weaponId)
    {
        Player.inst.EquipWeapon(Player.inst.GetWeapon(weaponId));
    }
}
