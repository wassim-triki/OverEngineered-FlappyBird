using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI highScoreUI; 
    [SerializeField] private TextMeshProUGUI newBadgeLabel; 
    

    void Awake()
    {
            score.OnHighScoreChanged += HandleNewHighScore;
            GameStateManager.OnMenu += HandleOnMenu;
    }
    
    private void OnDestroy()
    {
        if (score != null)
            score.OnHighScoreChanged -= HandleNewHighScore;
        if (GameStateManager.Instance != null)
            GameStateManager.OnMenu -= HandleOnMenu;
    }
    void HandleOnMenu()
    {
        newBadgeLabel.gameObject.SetActive(false);
    }
    void Start()
    {
        highScoreUI.text = score.High.ToString();
        newBadgeLabel.gameObject.SetActive(false);
    }




    private void HandleNewHighScore(int newHigh)
    {
            highScoreUI.text = newHigh.ToString();
            newBadgeLabel.gameObject.SetActive(true);
    }
}
