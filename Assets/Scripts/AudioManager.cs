using DefaultNamespace;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Library")]
    [SerializeField] private AudioLibrary library;

    private AudioSource _sfxSource;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.spatialBlend = 0f; // 2D
        }
    }

    //Play a sound effect by ID. Volume multiplies the library's default.
    public void Play(Sfx id, float volume = 1f)
    {
        if (!library) return;
        if (!library.TryGetRandom(id, out var clip, out var libVol)) return;

        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * libVol));
    }
}