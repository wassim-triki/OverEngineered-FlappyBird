// Assets/Scripts/AudioManager.cs
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
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float normalCutoffHz = 22000f;
    [SerializeField] private float underwaterCutoffHz = 2000f;
    [SerializeField] private float normalVolume = 0.4f;
    [SerializeField] private float underwaterVolume = 0.4f;
    [SerializeField] private float transition = 0.35f;

    [Header("Pitch (difficulty-driven while playing)")]
    [SerializeField] private float basePitch = 0.9f;
    [SerializeField] private float pitchRange = 0.2f;
    [SerializeField] private float pitchFollowSmooth = 0.25f;

    [Header("Underwater state (menu/pause/game over)")]
    [SerializeField] private float underwaterPitch = 0.85f;

    [Header("Score SFX pitch-up")]
    [SerializeField, Range(0.5f, 1.5f)] private float scorePitchMin = 1.00f;
    [SerializeField, Range(0.5f, 2.00f)] private float scorePitchMax = 1.35f;
    [SerializeField, Min(0f)]            private float scorePitchReturn = 0.12f;

    private Tween _sfxPitchTween;

    [Header("Difficulty")]
    [SerializeField] private DifficultyController difficultyController;

    private AudioSource _sfxSource;
    private AudioLowPassFilter _lp;
    private AudioReverbFilter _reverb;
    private Tween _musicFxTween;

    private bool _followPitch;
    private float _pitch;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.spatialBlend = 0f;

        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        if (musicSource != null)
        {
            _lp = musicSource.gameObject.GetComponent<AudioLowPassFilter>();
            if (_lp == null) _lp = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            _lp.lowpassResonanceQ = 1f;
            _lp.cutoffFrequency = normalCutoffHz;

            _reverb = musicSource.gameObject.GetComponent<AudioReverbFilter>();
            if (_reverb == null) _reverb = musicSource.gameObject.AddComponent<AudioReverbFilter>();
            _reverb.reverbPreset = AudioReverbPreset.Off;
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
        if (musicSource)
        {
            musicSource.pitch  = basePitch;
            musicSource.volume = normalVolume;
        }
    }

    void Update()
    {
        if (_followPitch && musicSource)
        {
            float target = TargetNormalPitch();
            float k = 1f - Mathf.Exp(-Time.unscaledDeltaTime / Mathf.Max(0.001f, pitchFollowSmooth));
            _pitch = Mathf.Lerp(musicSource.pitch, target, k);
            musicSource.pitch = _pitch;
        }
    }

    public void Play(Sfx id, float volume = 1f)
    {
        if (!library) return;
        if (!library.TryGetRandom(id, out var clip, out var libVol)) return;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * libVol));
    }

    private void HandlePlayStartOrResume()
    {
        ClearUnderwaterFX();
        _followPitch = true;
        if (musicSource) _pitch = musicSource.pitch;
    }

    private void ApplyUnderwaterFX()
    {
        if (!musicSource) return;

        _followPitch = false;
        _musicFxTween?.Kill();
        if (_reverb) _reverb.reverbPreset = AudioReverbPreset.Underwater;

        _musicFxTween = DOTween.Sequence()
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, underwaterCutoffHz, transition))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, underwaterPitch,   transition))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, underwaterVolume,  transition))
            .SetUpdate(true);
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
                _followPitch = true;
                _pitch = musicSource.pitch;
            });
    }

    public void PulseUnderwater(float inDuration, float holdDuration, float outDuration, Ease inEase, Ease outEase)
    {
        if (!musicSource) return;

        _followPitch = false;
        _musicFxTween?.Kill();
        if (_reverb) _reverb.reverbPreset = AudioReverbPreset.Underwater;

        float backPitch = TargetNormalPitch();

        _musicFxTween = DOTween.Sequence()
            // In
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, underwaterCutoffHz, Mathf.Max(0.01f, inDuration)).SetEase(inEase))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, underwaterPitch,   Mathf.Max(0.01f, inDuration)).SetEase(inEase))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, underwaterVolume,  Mathf.Max(0.01f, inDuration)).SetEase(inEase))
            // Hold
            .AppendInterval(Mathf.Max(0f, holdDuration))
            // Out
            .Append(DOTween.Sequence()
                .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, normalCutoffHz, Mathf.Max(0.01f, outDuration)).SetEase(outEase))
                .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, backPitch,    Mathf.Max(0.01f, outDuration)).SetEase(outEase))
                .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, normalVolume, Mathf.Max(0.01f, outDuration)).SetEase(outEase))
            )
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (_reverb) _reverb.reverbPreset = AudioReverbPreset.Off;
                _followPitch = true;
                _pitch = musicSource.pitch;
            });
    }

    private float TargetNormalPitch()
    {
        float t = 0f;
        if (difficultyController != null)
        {
            float min = difficultyController.minSpeed;
            float max = difficultyController.maxSpeed;
            float cur = difficultyController.CurrentSpeed;
            t = Mathf.InverseLerp(min, max, cur);
        }
        return basePitch + t * pitchRange;
    }

    public void PlayScoreUp()
    {
        if (!library) return;
        if (!library.TryGetRandom(Sfx.Score, out var clip, out var libVol)) return;

        float t = 0f;
        if (difficultyController != null)
            t = Mathf.Clamp01(difficultyController.Difficulty01);

        float pitch = Mathf.Lerp(scorePitchMin, scorePitchMax, t);

        _sfxPitchTween?.Kill();
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(libVol));

        _sfxPitchTween = DOTween
            .To(() => _sfxSource.pitch, x => _sfxSource.pitch = x, 1f, scorePitchReturn)
            .SetUpdate(true);
    }
}
