using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public TMP_Text timerText;

    private float timer;
    private bool isTiming = false;
    private int currentGateIndex = 0;
    private int totalGates;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        timer = 0f;
        timerText.text = "0:00.00";
        totalGates = FindObjectsOfType<TimeGate>().Length;
    }

    private void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = FormatTime(timer);
        }
    }

    public void CheckGate(int gateIndex)
    {
        if (gateIndex == currentGateIndex)
        {
            if (gateIndex == 0)
            {
                StartTimer();
            }

            currentGateIndex++;

            if (currentGateIndex == totalGates)
            {
                StopTimer();
            }
        }
    }

    private void StartTimer()
    {
        isTiming = true;
        timer = 0f;
    }

    private void StopTimer()
    {
        isTiming = false;
        Debug.Log("Timer stopped at: " + FormatTime(timer));
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0}:{1:00.00}", minutes, seconds);
    }
}
