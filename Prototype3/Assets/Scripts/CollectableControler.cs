using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CollectableControler : MonoBehaviour
{
    public GameObject[] collectables;
    public Text collectableCounterText;
    public string winScene = "WinScene";

    private int numOfCollectables = 0;
    private int collectablesCollected = 0;

    void Start()
    {
        numOfCollectables = collectables.Length;
        collectableCounterText.text = "0/" + numOfCollectables;
    }
    public void Collected() {
        collectablesCollected++;
        collectableCounterText.text = collectablesCollected + "/" + numOfCollectables;
        if (collectablesCollected >= numOfCollectables) {
            SceneManager.LoadScene(winScene);
        }
    }
}
