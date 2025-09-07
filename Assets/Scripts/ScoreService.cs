using System;
using UnityEngine;

public class ScoreService : MonoBehaviour

{
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;
    public int Current { get; private set; }
    public int High { get; private set; }


    private void Awake()
    {
        High = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void ResetScore()
    {
        Current = 0;
        OnScoreChanged?.Invoke(Current);
    }

    public void Add(int delta)
    {
        if (delta == 0) return;
        Current = Mathf.Max(0, Current + delta);
        OnScoreChanged?.Invoke(Current);
        if (Current > High)
        {
            High = Current;
            OnHighScoreChanged?.Invoke(High);
            PlayerPrefs.SetInt("HighScore", High);
        }
    }
}
