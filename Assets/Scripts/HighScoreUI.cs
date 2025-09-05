using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreText; // Preferred (TMP)

    void Awake()
    {
        // Auto-wire references if not set in Inspector
    }

    void OnEnable()
    {
        if (score != null)
            score.OnHighScoreChanged += UpdateHighScore;
    }

    void Start()
    {
        // Initialize display with saved or current high score
        UpdateHighScore(score.High);
    }

    void OnDestroy()
    {
        if (score != null)
            score.OnHighScoreChanged -= UpdateHighScore;
    }

    private void UpdateHighScore(int newHigh)
    {
        if (scoreText != null)
            scoreText.text = newHigh.ToString();
    }
}
