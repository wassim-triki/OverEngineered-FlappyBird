using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CollectibleEffect : ScriptableObject
    {
        // Apply effect to the collector (usually the player root or its Lives component owner)
        public abstract void Apply(GameObject collector);
    }
}
