// Assets/Scripts/WindAudio.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WindAudio : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] private AudioClip windLoop;
    [SerializeField, Range(0f, 1f)] private float baseVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float maxVolume  = 1f;

    [Header("Fall ramp")]
    [SerializeField] private float maxFallSpeedForMax = 10f; // fall speed where we hit maxVolume
    [SerializeField] private float smoothTime = 0.10f;       // seconds (fade toward target)

    private Rigidbody2D _rb;
    private AudioSource _src;
    private float _volSmoothed;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _src = gameObject.AddComponent<AudioSource>();
        _src.clip = windLoop;
        _src.loop = true;
        _src.playOnAwake = false;
        _src.spatialBlend = 0f; // 2D
        _src.volume = 0f;       // we’ll ramp to base on enable
    }

    void OnEnable()
    {
        _volSmoothed = 0f;
        if (windLoop != null) _src.Play();
    }

    void OnDisable()
    {
        if (_src != null) _src.Stop();
    }

    void FixedUpdate()
    {
        if (_rb == null || _src == null || _src.clip == null) return;

        // Downward speed: 0 when rising / hovering, positive when falling.
        float downSpeed = Mathf.Max(0f, -_rb.linearVelocity.y);

        // Map 0..maxFallSpeedForMax => baseVolume..maxVolume
        float t = Mathf.Clamp01(maxFallSpeedForMax <= 0f ? 1f : downSpeed / maxFallSpeedForMax);
        float target = Mathf.Lerp(baseVolume, maxVolume, t);

        // Smooth toward target (one-pole expo)
        float k = 1f - Mathf.Exp(-Time.fixedDeltaTime / Mathf.Max(0.001f, smoothTime));
        _volSmoothed = Mathf.Lerp(_volSmoothed, target, k);

        _src.volume = _volSmoothed;
    }
}