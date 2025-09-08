// Assets/Scripts/IdleBob.cs
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class IdleBob : MonoBehaviour
{
    [Header("Bobbing")]
    [SerializeField, Min(0f)] private float amplitude = 0.25f; // world units
    [SerializeField, Min(0.1f)] private float period = 1.8f;   // seconds for a full up/down
    [Header("Subtle tilt")]
    [SerializeField, Min(0f)] private float tiltDegrees = 5f;  // max +/- Z

    private float _startY;
    private Quaternion _startRot;
    private Tween _bobTween, _tiltTween;

    void Awake()
    {
        _startY = transform.localPosition.y;
        _startRot = transform.localRotation;
    }

    void OnEnable()
    {
        GameStateManager.OnMenu        += StartBob;
        GameStateManager.OnGamePaused  += StopBob;
        GameStateManager.OnGameStarted += StopBob;
        GameStateManager.OnGameResumed += StopBob;
        GameStateManager.OnGameOver    += StopBob;
    }

    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnMenu        -= StartBob;
            GameStateManager.OnGamePaused  -= StopBob;
            GameStateManager.OnGameStarted -= StopBob;
            GameStateManager.OnGameResumed -= StopBob;
            GameStateManager.OnGameOver    -= StopBob;
        }
        KillTweens(reset:true);
    }

    void Start()
    {
        // If we enter a scene already in Menu/Paused, kick it off.
        var s = GameStateManager.Instance?.CurrentState ?? GameState.Menu;
        if (s == GameState.Menu || s == GameState.Paused) StartBob();
    }

    void StartBob()
    {
        KillTweens(reset:false);

        // Vertical bob (local Y only)
        _bobTween = transform.DOLocalMoveY(_startY + amplitude, period * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true); // keep running if timeScale==0 (pause)

        // Gentle tilt around Z
        if (tiltDegrees > 0f)
        {
            _tiltTween = transform.DOLocalRotate(new Vector3(0f, 0f, tiltDegrees), period * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
    }

    void StopBob()
    {
        KillTweens(reset:true);
    }

    void KillTweens(bool reset)
    {
        _bobTween?.Kill();  _bobTween = null;
        _tiltTween?.Kill(); _tiltTween = null;

        if (reset)
        {
            var p = transform.localPosition;
            p.y = _startY;
            transform.localPosition = p;
            transform.localRotation = _startRot;
        }
    }
}
