using System;
using DefaultNamespace;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour, IDamageable
{
    private Lives _lives;

    private void Awake()
    {
        _lives = GetComponent<Lives>();
    }

    public void TakeDamage(int amount, DamageContext ctx = null)
    {
        if (_lives == null || !_lives.IsAlive) return;
        _lives.LoseLife(amount,ctx);
    }
}
