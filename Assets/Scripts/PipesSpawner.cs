using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    
    
    private float _timer = 0;
    private float _interval;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interval = difficulty ? difficulty.CurrentInterval : 2f;
        Spawn();
    }

    // Update is called once per frame
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
        if (min > max) { float mid = (spawnMinY + spawnMaxY)*0.5f; min = max = mid; }
        float centerY = UnityEngine.Random.Range(min, max);
        Vector3 spawnPos = new Vector3(transform.position.x, centerY, 0f);
        PipePair pair = Instantiate(pipePairPrefab, spawnPos, Quaternion.identity);
        pair.Initialize(scoreService,difficulty);
        pair.SetGap(gap);
    }
    
    public void Freeze() => enabled = false;

}
