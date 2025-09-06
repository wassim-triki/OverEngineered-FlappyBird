using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Sprites & Prefab")]
    [SerializeField] private Sprite[] cloudSprites;   // your 4 variants
    [SerializeField] private Cloud cloudPrefab;       // prefab with SpriteRenderer + Cloud

    [Header("Spawn Interval (seconds)")]
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(1.2f, 2.0f);

    [Header("Movement Speed (units/sec)")]
    [SerializeField] private Vector2 speedRange = new Vector2(1.8f, 2.6f); // min..max

    [Header("Spawn Y (world)")]
    [SerializeField] private Vector2 spawnYRange = new Vector2(-1.0f, 3.5f);

    [Header("Look")]
    [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.35f);
    // [SerializeField, Range(0f, 1f)] private float minAlpha = 0.6f;
    // [SerializeField, Range(0f, 1f)] private float maxAlpha = 1.0f;

    [Header("Sorting (within 'Clouds' layer)")]
    [SerializeField] private string cloudsSortingLayerName = "Clouds";
    [SerializeField] private int frontOrder = 10;   // small/near -> higher order
    [SerializeField] private int backOrder  = -20;  // big/far   -> lower order

    [Header("Screen edges")]
    [SerializeField] private Camera cam;
    [SerializeField] private float spawnXOffset   = 2f; // off-screen right
    [SerializeField] private float despawnXOffset = 2f; // off-screen left

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

        // Scale first (we'll derive speed + sorting from it)
        float scaleMin = Mathf.Min(scaleRange.x, scaleRange.y);
        float scaleMax = Mathf.Max(scaleRange.x, scaleRange.y);
        float scale    = Random.Range(scaleMin, scaleMax);
        cloud.transform.localScale = Vector3.one * scale;

        // 0 (small) .. 1 (big)
        float t = Mathf.InverseLerp(scaleMin, scaleMax, scale);

        // Look
        var sr = cloud.SR;
        sr.sprite           = cloudSprites[Random.Range(0, cloudSprites.Length)];
        sr.sortingLayerName = cloudsSortingLayerName;
        // Bigger -> further back (lower order)
        sr.sortingOrder     = Mathf.RoundToInt(Mathf.Lerp(frontOrder, backOrder, t));

        // var col = sr.color;
        // float aMin = Mathf.Min(minAlpha, maxAlpha);
        // float aMax = Mathf.Max(minAlpha, maxAlpha);
        // col.a = Random.Range(aMin, aMax);
        // sr.color = col;

        // Motion: Bigger -> Slower (map big scale to speedRange.min)
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;

        Gizmos.color = new Color(1,1,1,0.35f);
        var left  = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        var right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        Vector3 a = new Vector3(left.x,  spawnYRange.x, 0f);
        Vector3 b = new Vector3(right.x, spawnYRange.x, 0f);
        Vector3 c2= new Vector3(left.x,  spawnYRange.y, 0f);
        Vector3 d = new Vector3(right.x, spawnYRange.y, 0f);
        Gizmos.DrawLine(a, b); Gizmos.DrawLine(c2, d); Gizmos.DrawLine(a, c2); Gizmos.DrawLine(b, d);
    }
#endif
}
