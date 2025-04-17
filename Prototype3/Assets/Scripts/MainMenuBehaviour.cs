/******************************************************************************
 * Author: Brad Dixon
 * File Name: MainMenuBehaviour.cs
 * Creation Date: 4/3/2025
 * Brief: Contains code for menu buttons
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Slider sensSlider;

    public static MainMenuBehaviour Instance;
    public static float sensitivity;

    /// <summary>
    /// Ensures there is only one instance
    /// </summary>
    private void Awake()
    {
        //Makes sure there is one singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        LoadSensitivity();
    }

    /// <summary>
    /// Sets the sensitivity of the player
    /// </summary>
    /// <param name="slider"></param>
    public void SetSensitivity()
    {
        sensitivity = sensSlider.value;
        PlayerPrefs.SetFloat("sens", sensitivity);
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void LoadGameScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the playerPref of the sensitivity
    /// </summary>
    private void LoadSensitivity()
    {
        sensitivity = PlayerPrefs.GetFloat("sens");
        
        if (sensitivity < 10) {
            SetSensitivity();
        } else {
            sensSlider.value = sensitivity;
        }
    }

    /// <summary>
    /// Quits out of the game
    /// </summary>
    public void QuitGame()
    {
        //Quits out of the editor instead
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
