using System;
using DefaultNamespace;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Library (SFX)")]
    [SerializeField] private AudioLibrary library;

    [Header("Music (BGM)")]
    [SerializeField] private AudioSource musicSource;          // assign your soundtrack AudioSource here
    [SerializeField] private float normalCutoffHz = 22000f;
    [SerializeField] private float underwaterCutoffHz = 2000f; // muffled feel
    [SerializeField] private float normalVolume = 0.4f;
    [SerializeField] private float underwaterVolume = 0.4f;
    [SerializeField] private float transition = 0.35f;         // FX fade time

    [Header("Pitch (difficulty-driven while playing)")]
    [SerializeField] private float basePitch = 0.9f;           // pitch at easiest
    [SerializeField] private float pitchRange = 0.2f;          // added at hardest (=> base + range)
    [SerializeField] private float pitchFollowSmooth = 0.25f;  // seconds to settle toward target

    [Header("Underwater state (menu/pause/game over)")]
    [SerializeField] private float underwaterPitch = 0.85f;    // slight dip

    [Header("Score SFX pitch-up")]
    [SerializeField, Range(0.5f, 1.5f)] private float scorePitchMin = 1.00f;
    [SerializeField, Range(0.5f, 2.00f)] private float scorePitchMax = 1.35f;
    [SerializeField, Min(0f)]            private float scorePitchReturn = 0.12f;

    private Tween _sfxPitchTween;
    [Header("Difficulty")]
    [SerializeField] private DifficultyController difficultyController;

    private AudioSource _sfxSource;
    private AudioLowPassFilter _lp;
    private AudioReverbFilter _reverb; // optional seasoning
    private Tween _musicFxTween;

    // runtime
    private bool _followPitch;   // only true during gameplay
    private float _pitch;        // cache for smoothing

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // SFX source (kept on this GameObject)
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.spatialBlend = 0f; // 2D

        // Music filters live on the MUSIC source's object
        if (musicSource == null) musicSource = GetComponent<AudioSource>(); // fallback
        if (musicSource != null)
        {
            _lp = musicSource.gameObject.GetComponent<AudioLowPassFilter>();
            if (_lp == null) _lp = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            _lp.lowpassResonanceQ = 1f;
            _lp.cutoffFrequency = normalCutoffHz;

            _reverb = musicSource.gameObject.GetComponent<AudioReverbFilter>();
            if (_reverb == null) _reverb = musicSource.gameObject.AddComponent<AudioReverbFilter>();
            _reverb.reverbPreset = AudioReverbPreset.Off; // only for underwater feel here
        }
    }

    void OnEnable()
    {
        GameStateManager.OnGameOver   += ApplyUnderwaterFX;
        GameStateManager.OnMenu       += ApplyUnderwaterFX;
        GameStateManager.OnGamePaused += ApplyUnderwaterFX;

        GameStateManager.OnGameStarted += HandlePlayStartOrResume;
        GameStateManager.OnGameResumed += HandlePlayStartOrResume;
    }

    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnGameOver   -= ApplyUnderwaterFX;
            GameStateManager.OnMenu       -= ApplyUnderwaterFX;
            GameStateManager.OnGamePaused -= ApplyUnderwaterFX;

            GameStateManager.OnGameStarted -= HandlePlayStartOrResume;
            GameStateManager.OnGameResumed -= HandlePlayStartOrResume;
        }
    }

    void Start()
    {
        // Ensure starting pitch/volume reasonable if scene boots into menu
        if (musicSource)
        {
            musicSource.pitch  = basePitch;
            musicSource.volume = normalVolume;
        }
    }

    void Update()
    {
        // Follow difficulty -> pitch only in gameplay
        if (_followPitch && musicSource)
        {
            float target = TargetNormalPitch();
            // exponential smoothing (unscaled time so it feels stable across pauses)
            float k = 1f - Mathf.Exp(-Time.unscaledDeltaTime / Mathf.Max(0.001f, pitchFollowSmooth));
            _pitch = Mathf.Lerp(musicSource.pitch, target, k);
            musicSource.pitch = _pitch;
        }
    }

    // --- Public SFX ---
    public void Play(Sfx id, float volume = 1f)
    {
        if (!library) return;
        if (!library.TryGetRandom(id, out var clip, out var libVol)) return;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * libVol));
    }

    // --- Gameplay entry/resume ---
    private void HandlePlayStartOrResume()
    {
        ClearUnderwaterFX();   // lift filters first
        _followPitch = true;   // allow Update() to drive toward difficulty pitch
        if (musicSource) _pitch = musicSource.pitch;
    }

    // --- Music FX: underwater (menu/pause/game over) ---
    private void ApplyUnderwaterFX()
    {
        if (!musicSource) return;

        _followPitch = false; // DOTween owns pitch while underwater
        _musicFxTween?.Kill();
        if (_reverb) _reverb.reverbPreset = AudioReverbPreset.Underwater;

        _musicFxTween = DOTween.Sequence()
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, underwaterCutoffHz, transition))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, underwaterPitch,   transition))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, underwaterVolume,  transition))
            .SetUpdate(true); // run even if timeScale is 0
    }

    private void ClearUnderwaterFX()
    {
        if (!musicSource) return;

        _musicFxTween?.Kill();
        if (_reverb) _reverb.reverbPreset = AudioReverbPreset.Off;

        float targetPitch = TargetNormalPitch();

        _musicFxTween = DOTween.Sequence()
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, normalCutoffHz, transition * 0.7f))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, targetPitch,    transition * 0.7f))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, normalVolume,   transition * 0.7f))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _followPitch = true;                // hand control back to follower
                _pitch = musicSource.pitch;         // continue smoothly from here
            });
    }

    // --- Helpers ---
    private float TargetNormalPitch()
    {
        // Difficulty in [0..1] derived from speed ramp
        float t = 0f;
        if (difficultyController != null)
        {
            // Map current speed within its configured min..max
            float min = difficultyController.minSpeed;
            float max = difficultyController.maxSpeed;
            float cur = difficultyController.CurrentSpeed;
            t = Mathf.InverseLerp(min, max, cur);
        }
        // base + range * t
        return basePitch + t * pitchRange;
    }
    public void PlayScoreUp()
    {
        if (!library) return;
        if (!library.TryGetRandom(Sfx.Score, out var clip, out var libVol)) return;

        // Map difficulty 0..1 → pitch range
        float t = 0f;
        if (difficultyController != null)
            t = Mathf.Clamp01(difficultyController.Difficulty01);

        float pitch = Mathf.Lerp(scorePitchMin, scorePitchMax, t);

        // Apply pitch only for this one-shot, then ease back to 1f
        _sfxPitchTween?.Kill();
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(libVol));

        // Smoothly return pitch so other SFX aren't affected
        _sfxPitchTween = DG.Tweening.DOTween
            .To(() => _sfxSource.pitch, x => _sfxSource.pitch = x, 1f, scorePitchReturn)
            .SetUpdate(true);
    }
}
