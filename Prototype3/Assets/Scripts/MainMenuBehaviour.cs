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

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private string sceneName;

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void LoadGameScene()
    {
        SceneManager.LoadScene(sceneName);
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
