using System;
using UnityEngine;

public class PillarsSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private PillarPair pillarPairPrefab;
    [Header("Services")]
    [SerializeField] private ScoreService scoreService;
    [Header("Spawn")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float heightOffset = 9f;
    [SerializeField] private Vector2 gapRange = new Vector2(0f, 44f); // [min,max]
    
    
    private float _timer = 0;

    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled) return;
        _timer += Time.deltaTime;
        if (_timer >= spawnRate)
        {
            _timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        float gap = UnityEngine.Random.Range(gapRange.x, gapRange.y);
        
        float centerMin = transform.position.y - heightOffset + gap * 0.75f;
        float centerMax = transform.position.y + heightOffset - gap * 0.75f;
        float centerY = UnityEngine.Random.Range(centerMin, centerMax);
        Vector3 spawnPos = new Vector3(transform.position.x, centerY, 0f);
        PillarPair pair = Instantiate(pillarPairPrefab, spawnPos, Quaternion.identity);
        pair.Initialize(scoreService);
        pair.SetGap(gap);
    }
    
    public void Freeze() => enabled = false;

}
