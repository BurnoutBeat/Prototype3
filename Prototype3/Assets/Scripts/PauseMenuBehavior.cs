using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;

public class PauseMenuBehavior : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public string playScene = "FinalLevel";
    [Space(5)]
    [Header("MAIN_MENU")]
    public GameObject mainMenu;
    [Space(2)]
    public GameObject resumeButton;
    public GameObject howToPlayButton;
    public GameObject creditsButton;
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

    [Header("CREDITS")]
    public GameObject creditsMenu;
    [Space(2)]
    public Button creditsBack;

    public void EscapePressed()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            if (gameObject.activeSelf && mainMenu.activeSelf)
            {
                Resume();
            }
        }
        
        if (!mainMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            howToPlayMenu.SetActive(false);
            if (creditsMenu) { creditsMenu.SetActive(false); }
            mainMenu.SetActive(true);
            SelectFirstButton();
        }
        
    }
    public void LoadPlayScene() {
        SceneManager.LoadScene(playScene);
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
        SceneManager.LoadScene(mainMenuScene);
    }
    public void UpdateSensitivity()
    {
        PlayerPrefs.SetFloat("sens", sensitivitySlider.value);
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
    public void Credits()
    {
        creditsMenu.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(DelayByFrame(creditsBack.gameObject));
    }
    public void QuitGmae()
    {
        Application.Quit();
    }
}   
