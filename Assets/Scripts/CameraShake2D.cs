using System.Collections;
using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    [Header("Defaults (tweak to taste)")]
    [SerializeField] private float defaultDuration  = 0.14f;
    [SerializeField] private float defaultAmplitude = 0.15f;  // world units
    [SerializeField] private float defaultFrequency = 33f;    // Hz
    [SerializeField] private float rotationPerUnit  = 8f;     // deg per 1.0 amplitude

    Vector3 _originLocalPos;
    Quaternion _originLocalRot;
    float _seedX, _seedY, _seedR;
    Coroutine _routine;

    void Awake()
    {
        _originLocalPos = transform.localPosition;
        _originLocalRot = transform.localRotation;
        _seedX = Random.value * 100f;
        _seedY = Random.value * 100f;
        _seedR = Random.value * 100f;
    }

    public void ShakeLight() => Shake(defaultAmplitude, defaultFrequency, defaultDuration);

    public void ShakeHeavy() => Shake(defaultAmplitude * 1.4f, defaultFrequency * 0.8f, defaultDuration * 1.6f);

    public void Shake(float amplitude, float frequency, float duration)
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(DoShake(amplitude, frequency, duration));
    }

    IEnumerator DoShake(float amp, float freq, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            float envelope = 1f - u; envelope *= envelope; // ease-out^2

            float time = Time.time * freq;
            float nx = (Mathf.PerlinNoise(_seedX, time) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(_seedY, time) - 0.5f) * 2f;
            float nr = (Mathf.PerlinNoise(_seedR, time) - 0.5f) * 2f;

            Vector3 offset = new Vector3(nx, ny, 0f) * (amp * envelope);
            float rotZ = nr * (amp * rotationPerUnit) * envelope;

            transform.localPosition = _originLocalPos + offset;
            transform.localRotation = _originLocalRot * Quaternion.Euler(0f, 0f, rotZ);

            yield return null;
        }

        transform.localPosition = _originLocalPos;
        transform.localRotation = _originLocalRot;
        _routine = null;
    }
}
