using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cloud : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public SpriteRenderer SR => spriteRenderer;

    float _speed;
    float _despawnX;
    bool _initialized;

    void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called by the spawner
    public void Init(float speed, float despawnX)
    {
        _speed = speed;
        _despawnX = despawnX;
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;

        transform.position += Vector3.left * _speed * Time.deltaTime;

        if (transform.position.x <= _despawnX)
            Destroy(gameObject);
    }
}