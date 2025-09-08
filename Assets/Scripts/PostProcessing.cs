using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PostProcessing : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Volume postProcessVolume;

    private LensDistortion _lensDistortion;
    private ChromaticAberration _chromaticAberration;
    private Vignette _vignette;

    [Header("Init Targets (Menu / Paused / GameOver)")]
    [Range(-1f, 1f)] [SerializeField] private float initLensDistortionIntensity = 0.20f;
    [Range(0f, 1f)]  [SerializeField] private float initVignetteIntensity       = 0.35f;

    [Header("Play Targets (During Gameplay)")]
    [Range(-1f, 1f)] [SerializeField] private float lensDistortionIntensity      = -0.30f;
    [Range(0f, 1f)]  [SerializeField] private float chromaticAberrationIntensity = 0.25f;
    [Range(0f, 1f)]  [SerializeField] private float vignetteIntensity            = 0.25f;

    [Header("Camera Zoom (Orthographic)")]
    [SerializeField] private float initOrthoSize = 8.5f;
    [SerializeField] private float playOrthoSize = 9.5f;

    [Header("Tween")]
    [Min(0.05f)]     [SerializeField] private float duration = 0.35f;
    [SerializeField] private Ease  ease                      = Ease.OutCubic;
    [SerializeField] private bool  ignoreTimeScale           = true;

    private Tween _lensTween, _chromTween, _vignetteTween, _camTween;
    private Camera _cam;

    void Start()
    {
        if (!postProcessVolume)
            postProcessVolume = GetComponent<Volume>();

        if (!postProcessVolume || !postProcessVolume.profile)
        {
            Debug.LogWarning("[PostProcessing] Missing Volume or Profile.");
            enabled = false;
            return;
        }

        // Fetch overrides and enable intensity control
        if (postProcessVolume.profile.TryGet(out _lensDistortion))
        {
            _lensDistortion.active = true;
            _lensDistortion.intensity.overrideState = true;
        }
        if (postProcessVolume.profile.TryGet(out _chromaticAberration))
        {
            _chromaticAberration.active = true;
            _chromaticAberration.intensity.overrideState = true;
        }
        if (postProcessVolume.profile.TryGet(out _vignette))
        {
            _vignette.active = true;
            _vignette.intensity.overrideState = true;
        }

        // Camera: grab main (tagged), fallback to any camera
        _cam = Camera.main;
        if (!_cam) _cam = FindObjectOfType<Camera>();

        // Initial look
        SetInstant(initLensDistortionIntensity, 0f, initVignetteIntensity);
        SetCamInstant(initOrthoSize);
    }

    void OnEnable()
    {
        GameStateManager.OnMenu        += HandleMenu;
        GameStateManager.OnGameStarted += HandleGameStarted;
        GameStateManager.OnGameResumed += HandleGameStarted;
        GameStateManager.OnGameOver    += HandleGameOver;
        GameStateManager.OnGamePaused  += HandlePaused;
    }

    void OnDisable()
    {
        GameStateManager.OnMenu        -= HandleMenu;
        GameStateManager.OnGameStarted -= HandleGameStarted;
        GameStateManager.OnGameResumed -= HandleGameStarted;
        GameStateManager.OnGameOver    -= HandleGameOver;
        GameStateManager.OnGamePaused  -= HandlePaused;

        _lensTween?.Kill();
        _chromTween?.Kill();
        _vignetteTween?.Kill();
        _camTween?.Kill();
    }

    // --- State handlers ---
    void HandleMenu()
    {
        TweenTo(initLensDistortionIntensity, 0f, initVignetteIntensity);
        TweenCam(initOrthoSize);
    }

    void HandlePaused()
    {
        TweenTo(initLensDistortionIntensity, 0f, initVignetteIntensity);
        TweenCam(initOrthoSize);
    }

    void HandleGameStarted()
    {
        TweenTo(lensDistortionIntensity, chromaticAberrationIntensity, vignetteIntensity);
        TweenCam(playOrthoSize);
    }

    void HandleGameOver()
    {
        TweenTo(initLensDistortionIntensity, 0f, initVignetteIntensity);
        TweenCam(initOrthoSize);
    }

    // --- Helpers ---
    void SetInstant(float lens, float ca, float vig)
    {
        if (_lensDistortion)      _lensDistortion.intensity.value      = Mathf.Clamp(lens, -1f, 1f);
        if (_chromaticAberration) _chromaticAberration.intensity.value = Mathf.Clamp01(ca);
        if (_vignette)            _vignette.intensity.value            = Mathf.Clamp01(vig);
    }

    void TweenTo(float lensTarget, float caTarget, float vigTarget)
    {
        if (_lensDistortion)
        {
            _lensTween?.Kill();
            _lensTween = DOTween.To(
                    () => _lensDistortion.intensity.value,
                    v  => _lensDistortion.intensity.value = v,
                    Mathf.Clamp(lensTarget, -1f, 1f),
                    duration)
                .SetEase(ease)
                .SetUpdate(ignoreTimeScale);
        }

        if (_chromaticAberration)
        {
            _chromTween?.Kill();
            _chromTween = DOTween.To(
                    () => _chromaticAberration.intensity.value,
                    v  => _chromaticAberration.intensity.value = v,
                    Mathf.Clamp01(caTarget),
                    duration)
                .SetEase(ease)
                .SetUpdate(ignoreTimeScale);
        }

        if (_vignette)
        {
            _vignetteTween?.Kill();
            _vignetteTween = DOTween.To(
                    () => _vignette.intensity.value,
                    v  => _vignette.intensity.value = v,
                    Mathf.Clamp01(vigTarget),
                    duration)
                .SetEase(ease)
                .SetUpdate(ignoreTimeScale);
        }
    }

    void SetCamInstant(float ortho)
    {
        if (_cam && _cam.orthographic)
            _cam.orthographicSize = ortho;
    }

    void TweenCam(float orthoTarget)
    {
        if (!_cam || !_cam.orthographic) return;

        _camTween?.Kill();
        _camTween = DOTween.To(
                () => _cam.orthographicSize,
                v  => _cam.orthographicSize = v,
                orthoTarget,
                duration)
            .SetEase(ease)
            .SetUpdate(ignoreTimeScale);
    }
}
