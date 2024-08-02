using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the UI elements for the main menu.
/// </summary>
public class MenuUI : MonoBehaviour
{
    [Header("Screens")]
    public GameObject menuScreen;
    public GameObject playScreen;
    public GameObject optionsScreen;

    [Header("Levels")]
    public LevelDataScriptableObject[] levels;
    private bool[] levelsUnlocked;

    [Header("Buttons")]
    public Button[] levelButtons;

    [Header("Options")]
    public Slider volumeSlider;

    //Private Values
    private string sceneToLoad;

    //Displays the sent screen, while disabling all others.
    public void SetScreen (GameObject screen)
    {
        menuScreen.SetActive(false);
        playScreen.SetActive(false);
        optionsScreen.SetActive(false);

        screen.SetActive(true);
    }

    //Loads options from PlayerPrefs and sets them to the UI elements on the options screen.
    void LoadOptions ()
    {
        if(PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            volumeSlider.value = AudioListener.volume;
        }
    }

    //Called when the "Play" button on the menu screen is pressed.
    public void OnPlayButton ()
    {
        SetScreen(playScreen);
        LoadPlayScreenLevels();
    }

    //Called when the "Quit" button on the menu screen is pressed.
    public void OnQuitButton ()
    {
        Application.Quit();
    }

    //Called when the "Options" button (gear icon) on the menu screen is pressed.
    public void OnOptionsButton ()
    {
        SetScreen(optionsScreen);
        LoadOptions();
    }

    //Called when the "Reset Progress" button on the options screen is pressed.
    public void OnResetProgressButton ()
    {
        //Reset saved PlayerPrefs for all levels.
        for(int index = 0; index < levels.Length; ++index)
        {
            PlayerPrefs.SetInt("LevelCompleted_" + levels[index].sceneName, 0);
        }
    }

    //Called when a "Level" button on the play screen is pressed.
    public void OnLevelButton (int level)
    {
        if(levelsUnlocked[level])
        {
            sceneToLoad = levels[level].sceneName;

            //Fade out the screen.
            ScreenFade.inst.Fade(Color.black, 0.5f);

            //Invoke loading the scene in 1 second.
            Invoke("LoadTargetScene", 0.5f);
        }
    }

    //Loads the "sceneToLoad" scene.
    void LoadTargetScene ()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    //Called when the "Back" button on the play screen is pressed.
    public void OnBackButton ()
    {
        SetScreen(menuScreen);
    }

    //Called when the volume slider is changed.
    public void OnVolumeSliderChangeValue ()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);
    }

    //Loads up all the levels and displays them on the play screen.
    void LoadPlayScreenLevels ()
    {
        levelsUnlocked = new bool[levels.Length];

        //Find out if levels are unlocked or not.
        for(int x = 0; x < levels.Length; ++x)
        {
            bool unlocked = false;

            if(!levels[x].lockedAtStart)
                unlocked = true;
            else
            {
                unlocked = PlayerPrefs.GetInt("LevelCompleted_" + levels[x].levelNeededToCompleteToUnlock.sceneName) == 0 ? false : true;
            }

            levelsUnlocked[x] = unlocked;
        }

        //Loop through all the level buttons.
        for(int x = 0; x < levelButtons.Length; ++x)
        {
            levelButtons[x].gameObject.SetActive(false);

            //Is this button an actual level?
            if(x < levels.Length)
            {
                levelButtons[x].gameObject.SetActive(true);
                levelButtons[x].transform.Find("LevelName").GetComponent<Text>().text = levels[x].sceneName;

                //Either display locked icon or not, based on if the level is locked.
                if(levelsUnlocked[x])
                    levelButtons[x].transform.Find("Locked").gameObject.SetActive(false);
                else
                    levelButtons[x].transform.Find("Locked").gameObject.SetActive(true);
            }
        }
    }
}
