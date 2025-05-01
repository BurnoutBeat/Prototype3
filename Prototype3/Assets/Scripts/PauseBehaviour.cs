/******************************************************************************
 * Author: Brad Dixon
 * File Name: PauseBehaviour.cs
 * Creation Date: 4/29/2025
 * Brief: Loads the audio settings on scene start
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseBehaviour : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider mSlider, musicSlider, sSlider;

    /// <summary>
    /// Sets the audio sliders
    /// </summary>
    void Awake()
    {
        Slider[] sliders = FindObjectsOfType<Slider>();
        Debug.Log(sliders.Length);

        LoadVolume(mSlider, "master");
        LoadVolume(musicSlider, "music");
        LoadVolume(sSlider, "sfx");
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
}
