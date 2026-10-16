using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;

    double elapsedSeconds;
    long displayedSeconds = -1;

    void Awake()
    {
        UpdateDisplay();
    }

    void Update()
    {
        // Scaled game time is zero while the ESC menu pauses the game.
        elapsedSeconds += Time.deltaTime;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        long seconds = (long)Math.Floor(elapsedSeconds);
        if (seconds == displayedSeconds)
        {
            return;
        }

        displayedSeconds = seconds;
        // Total minutes keep counting past one hour instead of wrapping to zero.
        timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
    }
}
