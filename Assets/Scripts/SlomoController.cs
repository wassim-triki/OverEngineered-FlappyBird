// Assets/Scripts/SlomoController.cs
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public class SlomoController : MonoBehaviour
{
    [Header("Defaults")]
    [Range(0.1f, 1f)] [SerializeField] private float defaultTargetMultiplier = 0.55f;
    [SerializeField] private float defaultInDuration  = 0.22f;
    [SerializeField] private float defaultHoldDuration = 3.0f;   // << NEW (customizable)
    [SerializeField] private float defaultOutDuration = 0.90f;
    [SerializeField] private Ease  defaultInEase      = Ease.OutCubic;
    [SerializeField] private Ease  defaultOutEase     = Ease.InCubic;

    public float SpeedMultiplier { get; private set; } = 1f;

    private Tween _pulseTween;

    void OnEnable()
    {
        GameStateManager.OnMenu       += ResetInstant;
        GameStateManager.OnGameOver   += ResetInstant;
        GameStateManager.OnGamePaused += ResetInstant;
    }

    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnMenu       -= ResetInstant;
            GameStateManager.OnGameOver   -= ResetInstant;
            GameStateManager.OnGamePaused -= ResetInstant;
        }
        KillPulse();
    }

    void KillPulse()
    {
        _pulseTween?.Kill();
        _pulseTween = null;
    }

    void ResetInstant()
    {
        KillPulse();
        SpeedMultiplier = 1f;
    }

    /// <summary>
    /// Plays a slomo pulse: ease down -> HOLD -> ease up.
    /// If a pulse is active, it restarts from the current multiplier.
    /// </summary>
    public void PlayPulse(
        float targetMultiplier,
        float inDuration,
        float holdDuration,
        float outDuration,
        Ease inEase,
        Ease outEase)
    {
        KillPulse();

        targetMultiplier = Mathf.Clamp(targetMultiplier, 0.1f, 1f);

        var down = DOTween.To(() => SpeedMultiplier, x => SpeedMultiplier = x, targetMultiplier, Mathf.Max(0.01f, inDuration))
                          .SetEase(inEase)
                          .SetUpdate(true);

        var up = DOTween.To(() => SpeedMultiplier, x => SpeedMultiplier = x, 1f, Mathf.Max(0.01f, outDuration))
                        .SetEase(outEase)
                        .SetUpdate(true);

        _pulseTween = DOTween.Sequence()
            .Append(down)
            .AppendInterval(Mathf.Max(0f, holdDuration))
            .Append(up)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    // Convenience using defaults
    public void PlayPulseDefault()
    {
        PlayPulse(defaultTargetMultiplier, defaultInDuration, defaultHoldDuration, defaultOutDuration, defaultInEase, defaultOutEase);
    }
}
