using DefaultNamespace;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI newBadgeLabel;

    // Beat settings
    [Header("Beat Animation")]
    [SerializeField] private float beatScale = 1.15f;
    [SerializeField] private float beatUpDuration = 0.10f;
    [SerializeField] private float beatDownDuration = 0.18f;
    [SerializeField] private Ease beatUpEase = Ease.OutBack;
    [SerializeField] private Ease beatDownEase = Ease.OutCubic;

    private Vector3 _baseScale;
    private Tween _beatTween;

    private bool _newHighShownThisRun = false;
    private bool _scoreUIInitialized = false;

    void Start()
    {
        if (!scoreUI) { Debug.LogWarning("[ScoreUI] Missing scoreUI ref."); return; }

        _baseScale = scoreUI.rectTransform.localScale;

        newBadgeLabel.gameObject.SetActive(false);

        score.OnScoreChanged += UpdateScoreUI;
        score.OnHighScoreChanged += HandleHighScoreChanged;
        GameStateManager.OnMenu += HandleOnMenu;
        GameStateManager.OnGameOver += HandleGameOver;

        // Set initial value without beat
        scoreUI.text = score != null ? score.Current.ToString() : "0";
        _scoreUIInitialized = true;
    }

    void OnDestroy()
    {
        // Properly unsubscribe everything
        if (score != null)
        {
            score.OnScoreChanged -= UpdateScoreUI;
            score.OnHighScoreChanged -= HandleHighScoreChanged;
        }
        GameStateManager.OnMenu -= HandleOnMenu;
        GameStateManager.OnGameOver -= HandleGameOver;

        _beatTween?.Kill();
    }

    void HandleOnMenu()
    {
        _newHighShownThisRun = false;
        _scoreUIInitialized = false; // avoid beating on the first set after reset
        newBadgeLabel.gameObject.SetActive(false);
        ResetScoreScale();
    }

    void HandleGameOver()
    {
        _newHighShownThisRun = false;
        _scoreUIInitialized = false;
        newBadgeLabel.gameObject.SetActive(false);
        ResetScoreScale();
    }

    private void ResetScoreScale()
    {
        _beatTween?.Kill();
        if (scoreUI) scoreUI.rectTransform.localScale = _baseScale;
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
        if (!scoreUI) return;

        scoreUI.text = newScore.ToString();

        // Beat only on real changes during play (skip the very first init set)
        if (_scoreUIInitialized)
            PlayBeat();
        else
            _scoreUIInitialized = true;
    }

    private void PlayBeat()
    {
        var rt = scoreUI.rectTransform;

        _beatTween?.Kill();
        _beatTween = DOTween.Sequence()
            .Append(rt.DOScale(_baseScale * beatScale, beatUpDuration).SetEase(beatUpEase))
            .Append(rt.DOScale(_baseScale,            beatDownDuration).SetEase(beatDownEase));
    }
}
