using System;
using DefaultNamespace;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{

    public void TakeDamage(int amount, DamageContext ctx = null)
    {
        Destroy(gameObject);
    }
}
