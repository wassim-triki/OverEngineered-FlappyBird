using System;
using DefaultNamespace;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{

    public void TakeDamage(int amount, DamageContext ctx = null)
    {
        //Destroy pillar
    }
}
