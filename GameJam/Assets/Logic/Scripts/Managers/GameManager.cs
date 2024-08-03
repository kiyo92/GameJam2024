using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages general game states and times.
/// </summary>
public class GameManager : MonoBehaviour
{
    public CameraFollow cameraFollowSettings;
    public LevelDataScriptableObject level; //Level data file that relates to this scene.
    public int waveCountdownTime = 5;       //How long is the countdown before wave starts?
    public float curWaveTime;
    public int curWave;
    public bool isShowingBarrierFeedback;

    [Header("Bools")]
    public bool waveInProgress;

    [Header("Wave Start Conditions")]
    public bool refillHealthOnNewWave;
    public bool refillAmmoOnNewWave;

    //Instance
    public static GameManager inst;

    void Awake ()
    {
        inst = this;
    }

    void Start ()
    {
        StartGame();
    }

    void Update ()
    {
        if(waveInProgress)
            curWaveTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);
    }

    private void LateUpdate()
    {
        CheckBarrierFeedbackStatus();
    }

    public void CheckBarrierFeedbackStatus()
    {
        if (isShowingBarrierFeedback) {
            // TODO Chamar UI de aproximação de limites
        }
    }

    //Called when the game starts.
    public void StartGame ()
    {
        SetNextWave();
    }

    //Called to start spawning the next wave.
    public void SetNextWave ()
    {
        waveInProgress = true;
        ShopUI.inst.ToggleShop(false);
        curWaveTime = 0.0f;
        curWave = EnemySpawner.inst.nextWaveIndex + 1;

        GameUI.inst.StartCoroutine("SetWaveCountdownText", waveCountdownTime);
        Invoke("StartNextWave", waveCountdownTime + 1);
    }

    //Called after countdown is done from the method above.
    void StartNextWave ()
    {
        //If we refill health, do it.
        if(refillHealthOnNewWave)
            Player.inst.curHp = Player.inst.maxHp;

        //Same with ammo for all weapons.
        if(refillAmmoOnNewWave)
            foreach(Weapon weapon in Player.inst.weapons)
                Player.inst.RefillAmmo(weapon.id);

        EnemySpawner.inst.SetNewWave();
    }

    //Called when the wave is over.
    public void EndWave ()
    {
        waveInProgress = false;

        //Was this the last wave? Then we win!
        if(EnemySpawner.inst.nextWaveIndex == EnemySpawner.inst.waves.Length)
            WinGame();
        //Otherwise enable the next wave button which triggers the next wave.
        else
        {
            GameUI.inst.nextWaveButton.SetActive(true);
            GameUI.inst.openShopButton.SetActive(true);
        }
    }

    public void SetCameraLevel() {
        cameraFollowSettings.SetCameraLevel(level: 2);
    }

    //Called when all waves have been killed off.
    public void WinGame ()
    {
        // Show win screen
        //PlayerPrefs.SetInt("LevelCompleted_" + level.sceneName, 1);
        //GameUI.inst.SetEndGameText(true);
        //Invoke("ReturnToMenu", 5.0f);
    }

    //Called when the player dies.
    public void LoseGame ()
    {
        // Show menu game over
    }

    //Loads the menu scene.
    void ReturnToMenu ()
    {
        // Show initial menu
    }
}