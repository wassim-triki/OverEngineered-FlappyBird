using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Sprites & Prefab")]
    [SerializeField] private Sprite[] cloudSprites;   
    [SerializeField] private Cloud cloudPrefab;       

    [Header("Spawn Interval (seconds)")]
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(1.2f, 2.0f);

    [Header("Movement Speed (units/sec)")]
    [SerializeField] private Vector2 speedRange = new Vector2(1.8f, 2.6f); // min..max

    [Header("Spawn Y (world)")]
    [SerializeField] private Vector2 spawnYRange = new Vector2(-1.0f, 3.5f);

    [Header("Look")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.35f);

    [Header("Sorting (within 'Clouds' layer)")]
    [SerializeField] private string cloudsSortingLayerName = "Clouds";
    [SerializeField] private int frontOrder = 10;   
    [SerializeField] private int backOrder  = -20;  

    [Header("Screen edges")]
    [SerializeField] private Camera cam;
    [SerializeField] private float spawnXOffset   = 2f; 
    [SerializeField] private float despawnXOffset = 2f; 

    [Header("Limits")]
    [SerializeField] private int maxActive = 20;

    float _nextSpawnAt;

    void Start()
    {
        if (!cam) cam = Camera.main;
        ScheduleNext();
    }

    void Update()
    {
        if (Time.time >= _nextSpawnAt && transform.childCount < maxActive)
        {
            SpawnOne();
            ScheduleNext();
        }
    }

    void SpawnOne()
    {
        var right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        var left  = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));

        float x = right.x + spawnXOffset;
        float y = Random.Range(spawnYRange.x, spawnYRange.y);
        float despawnX = left.x - despawnXOffset;

        var cloud = Instantiate(cloudPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);

        float scaleMin = Mathf.Min(scaleRange.x, scaleRange.y);
        float scaleMax = Mathf.Max(scaleRange.x, scaleRange.y);
        float scale    = Random.Range(scaleMin, scaleMax);
        cloud.transform.localScale = Vector3.one * scale;

        float t = Mathf.InverseLerp(scaleMin, scaleMax, scale);

        var sr = cloud.SR;
        sr.sprite           = cloudSprites[Random.Range(0, cloudSprites.Length)];
        sr.sortingLayerName = cloudsSortingLayerName;
        // Bigger -> further back (lower order)
        sr.sortingOrder     = Mathf.RoundToInt(Mathf.Lerp(frontOrder, backOrder, t));


        // Motion: Bigger -> Slower
        float spMin = Mathf.Min(speedRange.x, speedRange.y);
        float spMax = Mathf.Max(speedRange.x, speedRange.y);
        float speed = Mathf.Lerp(spMax, spMin, t);

        cloud.Init(speed, despawnX);
    }

    void ScheduleNext()
    {
        float min = Mathf.Min(spawnIntervalRange.x, spawnIntervalRange.y);
        float max = Mathf.Max(spawnIntervalRange.x, spawnIntervalRange.y);
        _nextSpawnAt = Time.time + Random.Range(min, max);
    }

}
