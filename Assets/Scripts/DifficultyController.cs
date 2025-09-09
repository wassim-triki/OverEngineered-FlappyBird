// Assets/Scripts/DifficultyController.cs
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [Header("Driver (score-based)")]
    [SerializeField, Min(1)] private int rampScore = 50;
    [SerializeField] private ScoreService score;

    [Header("Toggles")]
    public bool enableGapScaling = true;
    public bool enableSpeedScaling = true;
    public bool enableSpawnScaling = true;

    [Header("Speed (world units/sec)")]
    public float minSpeed = 2f;
    public float maxSpeed = 4.5f;

    [Header("Spawn Interval (seconds)")]
    public float maxInterval = 2.0f;  // easy
    public float minInterval = 1.0f;  // hard

    [Header("Gap (units)")]
    public float maxGap = 20f;        // easy
    public float minGap = 10f;        // hard

    [Header("Slomo (optional)")]
    [SerializeField] private SlomoController slomo; // << NEW

    float T => Mathf.Clamp01(score ? (float)score.Current / rampScore : 0f);
    float SlomoM => slomo ? Mathf.Clamp(slomo.SpeedMultiplier, 0.1f, 1f) : 1f;

    public float CurrentSpeed
    {
        get
        {
            float baseSpeed = enableSpeedScaling ? Mathf.Lerp(minSpeed, maxSpeed, T) : minSpeed;
            return baseSpeed * SlomoM; // slow movement when slomo < 1
        }
    }

    public float CurrentInterval
    {
        get
        {
            float baseInterval = enableSpawnScaling ? Mathf.Lerp(maxInterval, minInterval, T) : maxInterval;
            return baseInterval * (1f / SlomoM);
        }
    }

    public float NextGap()
    {
        float gap = enableGapScaling ? Mathf.Lerp(maxGap, minGap, T) : maxGap;
        return Mathf.Clamp(gap, minGap, maxGap);
    }

    public float Difficulty01 => Mathf.Clamp01(score ? (float)score.Current / rampScore : 0f);

    public void ResetRun() {  }
}