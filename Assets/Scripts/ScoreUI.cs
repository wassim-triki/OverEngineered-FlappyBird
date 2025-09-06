using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI newBadgeLabel; 
    
    void Start()
    {
        newBadgeLabel.gameObject.SetActive(false);
        score.OnScoreChanged += UpdateScoreUI;
        score.OnHighScoreChanged += HandleHighScoreChanged;
        GameStateManager.OnMenu += HandleOnMenu;
        // Set initial value
        UpdateScoreUI(score.Current);
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (score != null)
            score.OnScoreChanged -= UpdateScoreUI;
            score.OnHighScoreChanged -= HandleHighScoreChanged;
            GameStateManager.OnMenu -= HandleOnMenu;
    }
    
    void HandleOnMenu()
    {
        newBadgeLabel.gameObject.SetActive(false);
    }

    private void HandleHighScoreChanged(int newHigh)
    {
        if(score.High>1)
            newBadgeLabel.gameObject.SetActive(true);
    }
    private void UpdateScoreUI(int newScore)
    {
        scoreUI.text = newScore.ToString();
    }
}
