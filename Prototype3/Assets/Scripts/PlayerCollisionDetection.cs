using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionDetection : MonoBehaviour
{
    public string sceneToLoad = "WinScene";
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player") { 
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
