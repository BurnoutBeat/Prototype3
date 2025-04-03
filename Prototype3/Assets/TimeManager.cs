using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float levelTimer;
    [SerializeField] private float personalBestLevelTimer;
    [SerializeField] private float highScoreLevelTimer;
    [SerializeField] private float gateTimer;
    [SerializeField] private float prevGateTimer;
    [SerializeField] private bool GateState = false;

    public TextMeshPro timerText; // Assign this in the Inspector
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;     
    }

    // Update is called once per frame
    void Update()
    {
        float levelTimer = Time.time - startTime; // Calculate the elapsed time
        // Update the display with the elapsed time (e.g., in seconds)
        timerText.text = levelTimer.ToString("0.00");
    }
}
