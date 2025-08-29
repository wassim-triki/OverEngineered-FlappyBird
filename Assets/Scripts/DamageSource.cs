using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class DamageSource : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField, Min(1)] private int damage = 1;
        
        [Header("Filtering")]
        [SerializeField] private LayerMask targetLayers = ~0;     // who we can hurt   
        [SerializeField] private bool ignoreSelfHierarchy = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;
            if (ignoreSelfHierarchy && other.gameObject.transform.IsChildOf(transform.root)) return;
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if(damageable == null) return;
            damageable.TakeDamage(damage, new DamageContext{Source = this.gameObject});
        }
    }
    
}