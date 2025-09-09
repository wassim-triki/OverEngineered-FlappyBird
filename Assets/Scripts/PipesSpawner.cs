// Assets/Scripts/PipesSpawner.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class PipesSpawner : MonoBehaviour
{
    [Header("Vertical Band (center & size, world units)")]
    [SerializeField] private float bandCenterY = 0f;
    [SerializeField, Min(0f)] private float bandHeight = 16f;

    [Header("Prefabs")]
    [SerializeField] private PipePair pipePairPrefab;

    [Header("Services")]
    [SerializeField] private ScoreService scoreService;
    [SerializeField] private DifficultyController difficulty;

    // ---------------------- Collectibles ----------------------
    [Header("Collectibles (optional)")]
    [SerializeField] private Collectible lifeCollectiblePrefab;
    [SerializeField] private Collectible healCollectiblePrefab;
    [SerializeField] private Collectible slomoCollectiblePrefab;

    [SerializeField, Range(0f, 1f)] private float lifeSpawnChance  = 0.08f;
    [SerializeField, Range(0f, 1f)] private float healSpawnChance  = 0.15f;
    [SerializeField, Range(0f, 1f)] private float slomoSpawnChance = 0.10f;

    [SerializeField, Min(0f)] private float gapInnerMargin = 0.75f;
    [SerializeField] private float xOffsetWithinPair = 0.8f;
    [SerializeField] private bool parentCollectibleToPipe = true;
    // ----------------------------------------------------------

    // Distance-based spawning (as you have)
    [Header("Distance Spawning (fallbacks when no Difficulty)")]
    [SerializeField] private float fallbackSpeedForSpacing   = 3.0f;
    [SerializeField] private float fallbackIntervalForTiming = 1.5f;

    private float _distanceSinceLast;

    void Start()
    {
        _distanceSinceLast = 0f;
        Spawn(); // initial
    }

    void Update()
    {
        if (!enabled) return;

        float speed         = CurrentSpeed();
        float targetSpacing = TargetWorldSpacing(); // speed * interval

        _distanceSinceLast += speed * Time.deltaTime;

        while (_distanceSinceLast >= targetSpacing)
        {
            _distanceSinceLast -= targetSpacing;
            Spawn();
            targetSpacing = TargetWorldSpacing();
        }
    }

    float CurrentSpeed()   => difficulty ? difficulty.CurrentSpeed   : Mathf.Max(0.01f, fallbackSpeedForSpacing);
    float CurrentInterval()=> difficulty ? difficulty.CurrentInterval: Mathf.Max(0.01f, fallbackIntervalForTiming);
    float TargetWorldSpacing() => CurrentSpeed() * CurrentInterval();

    void Spawn()
    {
        // Current gap size from difficulty (or prefab default)
        float gap = difficulty ? difficulty.NextGap() : pipePairPrefab.Gap;

        // Compute allowed Y range for the *gap center* so the full gap fits inside the band
        float halfBand = Mathf.Max(0f, bandHeight * 0.5f);
        float halfGap  = Mathf.Max(0f, gap * 0.5f);

        // If the band is too small for the gap, clamp to center (no crash/NaN)
        float minCenter = bandCenterY - (halfBand - halfGap);
        float maxCenter = bandCenterY + (halfBand - halfGap);
        if (minCenter > maxCenter)
        {
            minCenter = maxCenter = bandCenterY; // band < gap: just pin at center
        }

        float centerY  = UnityEngine.Random.Range(minCenter, maxCenter);
        Vector3 spawnPos = new Vector3(transform.position.x, centerY, 0f);

        PipePair pair = Instantiate(pipePairPrefab, spawnPos, Quaternion.identity);
        pair.Initialize(scoreService, difficulty);
        pair.SetGap(gap); // top/bottom anchors already move from this center

        TrySpawnOneCollectible(pair);
    }

    void TrySpawnOneCollectible(PipePair pair)
    {
        float gap = Mathf.Max(0f, pair.Gap);
        float halfGap  = gap * 0.5f;
        float halfSafe = Mathf.Max(0f, halfGap - gapInnerMargin);
        if (halfSafe <= 0f) return;

        var candidates = new List<Collectible>(3);
        if (lifeCollectiblePrefab  && UnityEngine.Random.value <= lifeSpawnChance)  candidates.Add(lifeCollectiblePrefab);
        if (healCollectiblePrefab  && UnityEngine.Random.value <= healSpawnChance)  candidates.Add(healCollectiblePrefab);
        if (slomoCollectiblePrefab && UnityEngine.Random.value <= slomoSpawnChance) candidates.Add(slomoCollectiblePrefab);
        if (candidates.Count == 0) return;

        var chosen = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        SpawnCollectible(chosen, pair, halfSafe);
    }

    void SpawnCollectible(Collectible prefab, PipePair pair, float halfSafe)
    {
        float y = pair.transform.position.y + UnityEngine.Random.Range(-halfSafe, halfSafe);
        float x = pair.transform.position.x + xOffsetWithinPair;

        Transform parent = parentCollectibleToPipe ? pair.transform : null;
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, parent);
    }

    public void Disable() => enabled = false;
    public void Enable()  => enabled = true;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Draw the vertical band and current valid center range (based on *prefab* gap for editor preview)
        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);

        float halfBand = Mathf.Max(0f, bandHeight * 0.5f);
        float yBottom  = bandCenterY - halfBand;
        float yTop     = bandCenterY + halfBand;

        var left  = transform.position.x - 8f; // arbitrary visual width
        var right = transform.position.x + 8f;

        Vector3 a = new Vector3(left,  yBottom, 0f);
        Vector3 b = new Vector3(right, yBottom, 0f);
        Vector3 c = new Vector3(left,  yTop,    0f);
        Vector3 d = new Vector3(right, yTop,    0f);
        Gizmos.DrawLine(a, b); Gizmos.DrawLine(c, d); Gizmos.DrawLine(a, c); Gizmos.DrawLine(b, d);

        // Preview safe center range for current prefab gap (editor hint)
        if (pipePairPrefab != null)
        {
            float gap     = pipePairPrefab.Gap;
            float halfGap = Mathf.Max(0f, gap * 0.5f);
            float minC    = bandCenterY - (halfBand - halfGap);
            float maxC    = bandCenterY + (halfBand - halfGap);
            if (minC <= maxC)
            {
                Gizmos.color = new Color(0f, 1f, 0.6f, 0.35f);
                Vector3 e = new Vector3(left,  minC, 0f);
                Vector3 f = new Vector3(right, minC, 0f);
                Vector3 g = new Vector3(left,  maxC, 0f);
                Vector3 h = new Vector3(right, maxC, 0f);
                Gizmos.DrawLine(e, f); Gizmos.DrawLine(g, h);
            }
        }
    }
#endif
}
