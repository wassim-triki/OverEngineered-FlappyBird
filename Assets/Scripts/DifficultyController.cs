using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [Header("Driver (score-based)")]
    [SerializeField, Min(1)] private int rampScore = 50; // score at which we hit max difficulty
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
    
    // t ∈ [0..1] based on score
    float T => Mathf.Clamp01(score ? (float)score.Current / rampScore : 0f);
    
    public float CurrentSpeed =>
        enableSpeedScaling ? Mathf.Lerp(minSpeed, maxSpeed, T) : minSpeed;
    
    public float CurrentInterval =>
        enableSpawnScaling ? Mathf.Lerp(maxInterval, minInterval, T) : maxInterval;
    
    public float NextGap()
    {
        float gap = enableGapScaling ? Mathf.Lerp(maxGap, minGap, T) : maxGap;
        return Mathf.Clamp(gap , minGap, maxGap);
    }
    
    // Normalized difficulty 0..1
    public float Difficulty01 => Mathf.Clamp01(score ? (float)score.Current / rampScore : 0f);

    
    public void ResetRun()
    {
        // nothing to do for now (score drives everything)
        // method here so GameManager can call it for symmetry
    }

}
