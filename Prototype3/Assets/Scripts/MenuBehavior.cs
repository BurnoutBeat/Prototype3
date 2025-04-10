using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    public string playScene = "PlayerMovement";
    public string MainMenu = "MainMenu";
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void GoToMenu() { 
        SceneManager.LoadScene(MainMenu);
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene(playScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
