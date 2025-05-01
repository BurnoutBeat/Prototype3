using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Audio;

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
    [SerializeField] AudioMixer mixer;

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

    private void Awake()
    {
        LoadSettings();
    }
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
    public void SetAudioSlider(Slider slider)
    {
        float volume = slider.value;

        string s = "";

        if (slider.name.Contains("Master"))
        {
            s = "master";
        }
        else if (slider.name.Contains("Music"))
        {
            s = "music";
        }
        else if (slider.name.Contains("SFX"))
        {
            s = "sfx";
        }

        mixer.SetFloat(s, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(s, volume);
    }

    private void LoadSettings()
    {
        LoadVolume(masterVolumeSlider, "master");
        LoadVolume(musicVolumeSlider, "music");
        LoadVolume(sfxVolumeSlider, "sfx");
        //Slider[] sliders = FindObjectsOfType<Slider>();
        //Debug.Log(sliders.Length);

        //foreach (Slider i in sliders)
        //{
        //    if (i.name.Contains("Master"))
        //    {
        //        if (PlayerPrefs.HasKey("master"))
        //        {
        //            LoadVolume(i, "master");
        //        }
        //        else
        //        {
        //            SetAudioSlider(i);
        //        }
        //    }
        //    else if (i.name.Contains("Music"))
        //    {
        //        if (PlayerPrefs.HasKey("music"))
        //        {
        //            LoadVolume(i, "music");
        //        }
        //        else
        //        {
        //            SetAudioSlider(i);
        //        }
        //    }
        //    else if (i.name.Contains("SFX"))
        //    {
        //        if (PlayerPrefs.HasKey("sfx"))
        //        {
        //            LoadVolume(i, "sfx");
        //        }
        //        else
        //        {
        //            SetAudioSlider(i);
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Loads the saved audio settings for the slider
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="s"></param>
    private void LoadVolume(Slider slider, string s)
    {
        float originalValue = slider.value;
        slider.value = PlayerPrefs.GetFloat(s);

        if (slider.value <= 0)
        {
            slider.value = originalValue;
        }
       mixer.SetFloat(s, Mathf.Log10(slider.value) * 20);
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
