using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreUI;
    
    
    void Start()
    {
        score.OnScoreChanged += UpdateScoreUI;
        // Set initial value
        UpdateScoreUI(score.Current);
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (score != null)
            score.OnScoreChanged -= UpdateScoreUI;
    }
    
    private void UpdateScoreUI(int newScore)
    {
        scoreUI.text = newScore.ToString();
    }
}
