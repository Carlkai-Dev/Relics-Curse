// GameTimer.cs
// Handles countdown timer logic, game over trigger, and UI display updates
// NOTES (future Carlkai):
// - Set `totalTime` in the Inspector (in seconds)
// - When time reaches 0, scene changes to "GameOverScene"
// - You can easily hook in a Game Over panel or sound trigger here too

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Set the total time in seconds")]
    public float totalTime = 120f; // Total countdown time

    public TMP_Text timerText; // Link your TMP text component in Inspector
    // public GameObject gameOverPanel; // Optional future use for UI instead of scene load

    private float timeRemaining;
    private bool timerRunning = true;

    void Start()
    {
        timeRemaining = totalTime;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                timerRunning = false;
                GameOver();
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void GameOver()
    {
        Debug.Log("Time's up! Game Over.");
        SceneManager.LoadScene("GameOverScene"); // Ensure this scene name matches
    }
}
