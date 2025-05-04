/******************************************************************************
 * Author: Brad Dixon
 * File Name: MainMenuBehaviour.cs
 * Creation Date: 4/3/2025
 * Brief: Contains code for menu buttons
 * ***************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Slider sensSlider;
    private Button firstSelectedButton;
    public static MainMenuBehaviour Instance;
    public static float sensitivity;
    public Button startButton;

    /// <summary>
    /// Ensures there is only one instance
    /// </summary>
    private void Start()
    {
        print("awake");
        LoadSensitivity();
        StartCoroutine(DelayByFrame(startButton.gameObject));
    }
    
    private IEnumerator DelayByFrame(GameObject objectToSelect)
    {
        print("called");
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);
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
}
