using DefaultNamespace;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI newBadgeLabel;

    [Header("Beat Animation")]
    [SerializeField] private float beatScale = 1.15f;
    [SerializeField] private float beatUpDuration = 0.10f;
    [SerializeField] private float beatDownDuration = 0.18f;
    [SerializeField] private Ease beatUpEase = Ease.OutBack;
    [SerializeField] private Ease beatDownEase = Ease.OutCubic;

    [Header("Menu Reveal")]
    [SerializeField] private float menuRevealDuration = 0.6f;
    [SerializeField] private float menuRevealDelay = 0.5f;
    [SerializeField] private Ease menuRevealEase = Ease.OutBack;

    private Vector3 _baseScale;
    private Tween _beatTween;

    private bool _newHighShownThisRun = false;
    private bool _initialRevealDone = false;
    private int  _lastShownScore = 0;

    void Start()
    {
        if (!scoreUI)
        {
            Debug.LogWarning("[ScoreUI] Missing scoreUI ref.");
            return;
        }

        _baseScale = scoreUI.rectTransform.localScale;

        if (newBadgeLabel) newBadgeLabel.gameObject.SetActive(false);

        _lastShownScore = score ? score.Current : 0;
        scoreUI.text = _lastShownScore.ToString();

        TryInitialMenuReveal();
    }

    void OnEnable()
    {
        if (score != null)
        {
            score.OnScoreChanged += UpdateScoreUI;
            score.OnHighScoreChanged += HandleHighScoreChanged;
        }

        GameStateManager.OnMenu += HandleOnMenu;
        GameStateManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        if (score != null)
        {
            score.OnScoreChanged -= UpdateScoreUI;
            score.OnHighScoreChanged -= HandleHighScoreChanged;
        }

        GameStateManager.OnMenu -= HandleOnMenu;
        GameStateManager.OnGameOver -= HandleGameOver;

        _beatTween?.Kill();
        _beatTween = null;
    }

    private void TryInitialMenuReveal()
    {
        if (_initialRevealDone) return;

        bool inMenu =
            GameStateManager.Instance != null &&
            GameStateManager.Instance.CurrentState == GameState.Menu;

        if (inMenu)
        {
            PlayMenuReveal();
            _initialRevealDone = true;
        }
    }

    private void HandleOnMenu()
    {
        _newHighShownThisRun = false;
        if (newBadgeLabel) newBadgeLabel.gameObject.SetActive(false);

        // sync last shown baseline to whatever score is on entering menu (likely 0)
        _lastShownScore = score ? score.Current : 0;

        PlayMenuReveal();
    }

    private void PlayMenuReveal()
    {
        if (!scoreUI) return;

        _beatTween?.Kill();

        var rt = scoreUI.rectTransform;
        rt.localScale = Vector3.zero;

        _beatTween = rt.DOScale(_baseScale, menuRevealDuration)
                       .SetEase(menuRevealEase)
                       .SetDelay(menuRevealDelay);
    }

    private void HandleGameOver()
    {
        _newHighShownThisRun = false;
        if (newBadgeLabel) newBadgeLabel.gameObject.SetActive(false);

        // reset baseline; prevents a beat on the 0 reset
        _lastShownScore = score ? score.Current : 0;

        ResetScoreScale();
    }

    private void ResetScoreScale()
    {
        _beatTween?.Kill();
        _beatTween = null;
        if (scoreUI) scoreUI.rectTransform.localScale = _baseScale;
    }

    private void HandleHighScoreChanged(int newHigh)
    {
        if (_newHighShownThisRun) return;
        _newHighShownThisRun = true;

        if (!newBadgeLabel) return;

        newBadgeLabel.gameObject.SetActive(true);

        newBadgeLabel.fontSize = 0f;
        var c = newBadgeLabel.color;
        newBadgeLabel.color = new Color(c.r, c.g, c.b, 0f);

        DOTween.To(() => newBadgeLabel.fontSize,
                   x => newBadgeLabel.fontSize = x,
                   25.5f,
                   0.5f)
               .SetEase(Ease.OutBack);

        newBadgeLabel.DOFade(0.7f, 0.5f).SetEase(Ease.OutBack);
    }

    private void UpdateScoreUI(int newScore)
    {
        if (!scoreUI) return;

        bool increased = newScore > _lastShownScore;

        scoreUI.text = newScore.ToString();

        if (increased)
            PlayBeat();

        _lastShownScore = newScore;
    }

    private void PlayBeat()
    {
        if (!scoreUI) return;

        var rt = scoreUI.rectTransform;

        _beatTween?.Kill();
        _beatTween = DOTween.Sequence()
            .Append(rt.DOScale(_baseScale * beatScale, beatUpDuration).SetEase(beatUpEase))
            .Append(rt.DOScale(_baseScale,            beatDownDuration).SetEase(beatDownEase));
    }
}
