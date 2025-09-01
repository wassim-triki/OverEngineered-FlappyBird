using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PipesSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private PipePair pipePairPrefab;
    [Header("Services")]
    [SerializeField] private ScoreService scoreService;
    [SerializeField] private DifficultyController difficulty;
    [Header("Spawn Geometry")]
    [SerializeField] private float heightOffset = 12f;
    
    
    
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

        float centerMin = transform.position.y - heightOffset + gap;
        float centerMax = transform.position.y + heightOffset - gap;
        float centerY = UnityEngine.Random.Range(centerMin, centerMax);
        Vector3 spawnPos = new Vector3(transform.position.x, centerY, 0f);
        PipePair pair = Instantiate(pipePairPrefab, spawnPos, Quaternion.identity);
        pair.Initialize(scoreService,difficulty);
        pair.SetGap(gap);
    }
    
    public void Freeze() => enabled = false;

}
