using System;
using UnityEngine;

namespace DefaultNamespace
{
    
    [RequireComponent(typeof(Collider2D))]
    public class DamageSource : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private bool fatal = false;         // ← ground: ON
        [SerializeField, Min(1)] private int damage = 1; 

        [Header("Filtering")]
        [SerializeField] private LayerMask targetLayers = ~0; // who we can hurt
        [SerializeField] private bool ignoreSelfHierarchy = true;


        void OnTriggerEnter2D(Collider2D other)  => TryHit(other);
        void OnCollisionEnter2D(Collision2D col) => TryHit(col.collider);

        void TryHit(Collider2D other)
        {
            // Checks if a GameObject is in this LayerMask.
            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
            if (ignoreSelfHierarchy && other.transform.IsChildOf(transform.root)) return;

            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null) return;

            var ctx = new DamageContext { Source = this.gameObject, IsFatal = fatal };
            damageable.TakeDamage(fatal ? int.MaxValue : damage, ctx);
        }
    }
    
}