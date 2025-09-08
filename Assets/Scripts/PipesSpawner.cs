using System;
using System.Collections.Generic;
using UnityEngine;

public class PipesSpawner : MonoBehaviour
{
    [Header("Vertical band (world Y)")]
    [SerializeField] private float spawnMinY = -8f;
    [SerializeField] private float spawnMaxY =  8f;

    [Header("Prefabs")]
    [SerializeField] private PipePair pipePairPrefab;

    [Header("Services")]
    [SerializeField] private ScoreService scoreService;
    [SerializeField] private DifficultyController difficulty;

    // ---------------------- Collectibles ----------------------
    [Header("Collectibles (optional)")]
    [SerializeField] private Collectible lifeCollectiblePrefab;
    [SerializeField] private Collectible healCollectiblePrefab;

    [Tooltip("Independent chances per pair; at most ONE collectible will spawn.")]
    [SerializeField, Range(0f, 1f)] private float lifeSpawnChance = 0.08f;
    [SerializeField, Range(0f, 1f)] private float healSpawnChance = 0.15f;

    [Tooltip("Safety margin inside the gap edges (world units) to avoid touching pipes).")]
    [SerializeField, Min(0f)] private float gapInnerMargin = 0.75f;

    [Tooltip("Horizontal offset from the pipe pair’s X (positive puts it slightly after the gate).")]
    [SerializeField] private float xOffsetWithinPair = 0.8f;

    [Tooltip("If true, spawned collectible is parented under PipePair so it inherits movement & cleanup.")]
    [SerializeField] private bool parentCollectibleToPipe = true;
    // ----------------------------------------------------------

    private float _timer = 0f;
    private float _interval;

    void Start()
    {
        _interval = difficulty ? difficulty.CurrentInterval : 2f;
        Spawn();
    }

    void Update()
    {
        if (!enabled) return;

        _timer += Time.deltaTime;
        if (_timer >= _interval)
        {
            _timer = 0f;
            Spawn();
            _interval = difficulty ? difficulty.CurrentInterval : _interval; // schedule next
        }
    }

    void Spawn()
    {
        float gap = difficulty ? difficulty.NextGap() : pipePairPrefab.Gap;

        float min = spawnMinY + gap;
        float max = spawnMaxY - gap;
        if (min > max)
        {
            float mid = (spawnMinY + spawnMaxY) * 0.5f;
            min = max = mid;
        }

        float centerY = UnityEngine.Random.Range(min, max);
        Vector3 spawnPos = new Vector3(transform.position.x, centerY, 0f);

        PipePair pair = Instantiate(pipePairPrefab, spawnPos, Quaternion.identity);
        pair.Initialize(scoreService, difficulty);
        pair.SetGap(gap);

        TrySpawnOneCollectible(pair);
    }

    void TrySpawnOneCollectible(PipePair pair)
    {
        // Compute safe Y band inside the gap once
        float gap = Mathf.Max(0f, pair.Gap);
        float halfGap  = gap * 0.5f;
        float halfSafe = Mathf.Max(0f, halfGap - gapInnerMargin);
        if (halfSafe <= 0f) return; // no safe space

        // Roll each candidate independently, then pick ONE at random if multiple passed.
        var candidates = new List<Collectible>(2);

        if (lifeCollectiblePrefab && UnityEngine.Random.value <= lifeSpawnChance)
            candidates.Add(lifeCollectiblePrefab);

        if (healCollectiblePrefab && UnityEngine.Random.value <= healSpawnChance)
            candidates.Add(healCollectiblePrefab);

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
        // Exactly one collectible spawned per pair.
    }

    public void Disable() => enabled = false;
    public void Enable()  => enabled = true;
}
