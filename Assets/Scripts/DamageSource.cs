using UnityEngine;
using DefaultNamespace;

[RequireComponent(typeof(Collider2D))]
public class DamageSource : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool fatal;         // ← ground: ON
    [SerializeField, Min(1)] private int damage = 1; 
    [SerializeField] private bool disableSelfOnHit = true;

    [Header("Filtering")]
    [SerializeField] private LayerMask targetLayers = ~0; // who we can hurt
    [SerializeField] private bool ignoreSelfHierarchy = true;
    
    private Collider2D _collider;

    private void Awake()
    {
        // Cache own collider to avoid repeated GetComponent calls on hit.
        _collider = GetComponent<Collider2D>();
    }
    
    public void ResetCollider()
    {
        if (_collider != null)
            _collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)  => TryHit(other);
    private void OnCollisionEnter2D(Collision2D col) => TryHit(col.collider);

    private void TryHit(Collider2D other)
    {
        if (other == null) return;

        // Checks if a GameObject is in this LayerMask.
        if (!IsInTargetLayers(other.gameObject)) return;
        if (ignoreSelfHierarchy && other.transform.IsChildOf(transform.root)) return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null) return;

        var ctx = new DamageContext { Source = this.gameObject, IsFatal = fatal };
        damageable.TakeDamage(fatal ? int.MaxValue : damage, ctx);
        
        // disable self collider to prevent multiple hits
        if (_collider != null && (disableSelfOnHit || fatal))
            _collider.enabled = false;
    }

    private bool IsInTargetLayers(GameObject go)
    {
        // True if the object's layer is included in the configured mask.
        return (targetLayers.value & (1 << go.layer)) != 0;
    }
}