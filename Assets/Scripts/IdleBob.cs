// Assets/Scripts/IdleBob.cs
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class IdleBob : MonoBehaviour
{
    [Header("Bobbing")]
    [SerializeField, Min(0f)]   private float amplitude   = 0.25f; // world units
    [SerializeField, Min(0.1f)] private float period      = 1.8f;  // seconds full cycle
    [Header("Subtle tilt")]
    [SerializeField, Min(0f)]   private float tiltDegrees = 5f;    // +/- Z around current

    private float _startY;
    private Tween _bobTween, _tiltTween;
    private Coroutine _startCo;

    void OnEnable()
    {
        GameStateManager.OnMenu        += StartBob;
        GameStateManager.OnGameStarted += StopBob;
        GameStateManager.OnGameResumed += StopBob;
        GameStateManager.OnGameOver    += StopBob;
    }

    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnMenu        -= StartBob;
            GameStateManager.OnGameStarted -= StopBob;
            GameStateManager.OnGameResumed -= StopBob;
            GameStateManager.OnGameOver    -= StopBob;
        }
        KillTweens();
        if (_startCo != null) { StopCoroutine(_startCo); _startCo = null; }
    }

    void Start()
    {
        var s = GameStateManager.Instance?.CurrentState ?? GameState.Menu;
        if (s == GameState.Menu) StartBob(); // scene boot into menu
    }

    public void StartBob()
    {
        // Ensure only one pending starter
        if (_startCo != null) StopCoroutine(_startCo);
        KillTweens();

        // Delay one frame so GameManager.ResetPlayer has run
        _startCo = StartCoroutine(StartBobNextFrame());
    }

    IEnumerator StartBobNextFrame()
    {
        yield return null; // wait a frame (after all listeners handle OnMenu)
        _startCo = null;

        // Sample current as baseline AFTER ResetPlayer()
        _startY = transform.localPosition.y;

        _bobTween = transform.DOLocalMoveY(_startY + amplitude, period * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        if (tiltDegrees > 0f)
        {
            _tiltTween = transform.DOBlendableLocalRotateBy(
                    new Vector3(0f, 0f, tiltDegrees),
                    period * 0.5f,
                    RotateMode.Fast)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
    }

    public void StopBob()
    {
        // Just stop; keep whatever pose/rotation the player currently has
        if (_startCo != null) { StopCoroutine(_startCo); _startCo = null; }
        KillTweens();
    }

    private void KillTweens()
    {
        _bobTween?.Kill();  _bobTween = null;
        _tiltTween?.Kill(); _tiltTween = null;
    }
}
