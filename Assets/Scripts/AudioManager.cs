using DefaultNamespace;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Library (SFX)")]
    [SerializeField] private AudioLibrary library;

    [Header("Music (BGM)")]
    [SerializeField] private AudioSource musicSource;         // ← assign your soundtrack AudioSource here
    [SerializeField] private float normalCutoffHz = 22000f;
    [SerializeField] private float underwaterCutoffHz = 700f; // ~“muffled” feel
    [SerializeField] private float normalPitch = 1f;
    [SerializeField] private float underwaterPitch = 0.95f;   // slight pitch dip
    [SerializeField] private float normalVolume = 1f;
    [SerializeField] private float underwaterVolume = 0.85f;  // small duck
    [SerializeField] private float transition = 0.35f;        // fade time

    private AudioSource _sfxSource;
    private AudioLowPassFilter _lp;
    private AudioReverbFilter _reverb; // optional seasoning
    private Tween _musicFxTween;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // SFX source (kept on this GameObject)
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.spatialBlend = 0f; // 2D
        }

        // Music filters live on the MUSIC source's object (not here) so SFX remain clean
        if (musicSource == null) musicSource = GetComponent<AudioSource>(); // fallback if you put it on the same object
        if (musicSource != null)
        {
            _lp = musicSource.gameObject.GetComponent<AudioLowPassFilter>();
            if (_lp == null) _lp = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            _lp.lowpassResonanceQ = 1f;
            _lp.cutoffFrequency = normalCutoffHz;

            _reverb = musicSource.gameObject.GetComponent<AudioReverbFilter>();
            if (_reverb == null) _reverb = musicSource.gameObject.AddComponent<AudioReverbFilter>();
            _reverb.reverbPreset = AudioReverbPreset.Off; // enable only on death
        }
    }

    void OnEnable()
    {
        GameStateManager.OnGameOver  += ApplyUnderwaterFX;
        GameStateManager.OnMenu      += ApplyUnderwaterFX;
        GameStateManager.OnGameStarted += ClearUnderwaterFX;
        GameStateManager.OnGameResumed += ClearUnderwaterFX;
        GameStateManager.OnGamePaused  += ApplyUnderwaterFX; // optional
    }

    void OnDisable()
    {
        GameStateManager.OnGameOver  -= ApplyUnderwaterFX;
        GameStateManager.OnMenu      -= ApplyUnderwaterFX;
        GameStateManager.OnGameStarted -= ClearUnderwaterFX;
        GameStateManager.OnGameResumed -= ClearUnderwaterFX;
        GameStateManager.OnGamePaused  -= ApplyUnderwaterFX;
    }

    // --- Public SFX ---
    public void Play(Sfx id, float volume = 1f)
    {
        if (!library) return;
        if (!library.TryGetRandom(id, out var clip, out var libVol)) return;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * libVol));
    }

    // --- Music FX ---
    private void ApplyUnderwaterFX()
    {
        if (!musicSource) return;

        _musicFxTween?.Kill();
        _reverb.reverbPreset = AudioReverbPreset.Underwater;

        _musicFxTween = DOTween.Sequence()
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, underwaterCutoffHz, transition))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, underwaterPitch,   transition))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, underwaterVolume,  transition))
            .SetUpdate(true); // run even if you later freeze timeScale
    }

    private void ClearUnderwaterFX()
    {
        if (!musicSource) return;

        _musicFxTween?.Kill();
        _reverb.reverbPreset = AudioReverbPreset.Off;

        _musicFxTween = DOTween.Sequence()
            .Join(DOTween.To(() => _lp.cutoffFrequency, x => _lp.cutoffFrequency = x, normalCutoffHz, transition * 0.7f))
            .Join(DOTween.To(() => musicSource.pitch,    x => musicSource.pitch    = x, normalPitch,    transition * 0.7f))
            .Join(DOTween.To(() => musicSource.volume,   x => musicSource.volume   = x, normalVolume,   transition * 0.7f))
            .SetUpdate(true);
    }
}
