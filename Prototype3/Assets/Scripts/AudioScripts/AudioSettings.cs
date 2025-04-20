/******************************************************************************
 * Author: Brad Dixon
 * File Name: AudioSettings.cs
 * Creation Date: 4/8/2025
 * Brief: Controls how loud the master, music, and sfx are
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private GameObject audioStuff;

    /// <summary>
    /// Sets the settings on startup and then makes it so the player sees the main menu instead
    /// </summary>
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            audioStuff.SetActive(true);
            LoadSettings();
            audioStuff.SetActive(false);
        }
    }

    /// <summary>
    /// Goes through each slider and sets the saved audio setting
    /// </summary>
    public void LoadSettings()
    {
        Slider[] sliders = FindObjectsOfType<Slider>();
        Debug.Log(sliders.Length);

        foreach (Slider i in sliders)
        {
            if (i.name.Contains("Master"))
            {
                if (PlayerPrefs.HasKey("master"))
                {
                    LoadVolume(i, "master");
                }
                else
                {
                    SetSlider(i);
                }
            }
            else if (i.name.Contains("Music"))
            {
                if (PlayerPrefs.HasKey("music"))
                {
                    LoadVolume(i, "music");
                }
                else
                {
                    SetSlider(i);
                }
            }
            else if (i.name.Contains("SFX"))
            {
                if (PlayerPrefs.HasKey("sfx"))
                {
                    LoadVolume(i, "sfx");
                }
                else
                {
                    SetSlider(i);
                }
            }
        }
    }

    /// <summary>
    /// Sets the volume when the slider gets changed
    /// </summary>
    /// <param name="slider"></param>
    public void SetSlider(Slider slider)
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

    /// <summary>
    /// Loads the saved audio settings for the slider
    /// </summary>
    /// <param name="slider"></param>
    /// <param name="s"></param>
    private void LoadVolume(Slider slider, string s)
    {
        slider.value = PlayerPrefs.GetFloat(s);

        mixer.SetFloat(s, Mathf.Log10(slider.value) * 20);
    }
}
