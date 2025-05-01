using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuBehavior : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    [Space(5)]
    [Header("MAIN_MENU")]
    public GameObject mainMenu;
    [Space(2)]
    public GameObject resumeButton;
    public GameObject howToPlayButton;
    public GameObject settingsButton;
    public GameObject returnToMenuButton;

    [Header("HOW_TO_PLAY")]
    public GameObject howToPlayMenu;
    [Space(2)]
    public Button howToPlayBack;

    [Header("SETTINGS")]
    public GameObject settingsMenu;
    [Space(2)]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider sensitivitySlider;
    public Button settingsBack;

    public void EscapePressed()
    {
        if (gameObject.activeSelf && mainMenu.activeSelf) {
            Resume();
        }
        if (settingsMenu.activeSelf || howToPlayMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            howToPlayMenu.SetActive(false);
            mainMenu.SetActive(true);
            SelectFirstButton();
        }
        
    }
    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    public void howToPlay()
    {
        howToPlayMenu.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(DelayByFrame(howToPlayBack.gameObject));
    }
    public void Settings()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(DelayByFrame(masterVolumeSlider.gameObject));
    }
    public void ReturnToMenu()
    {
        print("called");
        SceneManager.LoadScene(mainMenuScene);
    }
    public void SelectFirstButton() {
        StartCoroutine(DelayByFrame(resumeButton.gameObject));
    }
    private IEnumerator DelayByFrame(GameObject objectToSelect)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }
}   
