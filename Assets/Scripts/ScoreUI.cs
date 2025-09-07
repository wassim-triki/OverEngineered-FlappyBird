using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI newBadgeLabel;
    private float _newBadgeFontSize;
    private bool _newHighShownThisRun = false;
    void Start()
    {
        newBadgeLabel.gameObject.SetActive(false);
        score.OnScoreChanged += UpdateScoreUI;
        score.OnHighScoreChanged += HandleHighScoreChanged;
        GameStateManager.OnMenu += HandleOnMenu;
        GameStateManager.OnGameOver += HandleGameOver;
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
            GameStateManager.OnGameOver -= HandleGameOver;
    }

    void HandleOnMenu()
    {
        _newHighShownThisRun = false;
        newBadgeLabel.gameObject.SetActive(false);
    }
    void HandleGameOver()
    {
        _newHighShownThisRun = false;
        newBadgeLabel.gameObject.SetActive(false);
    }

    private void HandleHighScoreChanged(int newHigh)
    {
        if (_newHighShownThisRun) return;
        _newHighShownThisRun = true;
        newBadgeLabel.gameObject.SetActive(true);

        // Reset initial state
        newBadgeLabel.fontSize = 0f;
        var c = newBadgeLabel.color;
        newBadgeLabel.color = new Color(c.r, c.g, c.b, 0f);

        // Animate font size
        DOTween.To(() => newBadgeLabel.fontSize, 
                x => newBadgeLabel.fontSize = x, 
                25.5f, 
                0.5f)
            .SetEase(Ease.OutBack);

        // Animate opacity (alpha up to ~70%)
        newBadgeLabel.DOFade(0.7f, 0.5f).SetEase(Ease.OutBack);
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreUI.text = newScore.ToString();
    }
}
