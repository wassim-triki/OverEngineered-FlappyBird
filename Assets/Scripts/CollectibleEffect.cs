using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CollectibleEffect : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField] private Sfx sfxOnPickup = Sfx.Pickup;   // default

        public Sfx SfxOnPickup => sfxOnPickup;

        // Apply effect to the collector (usually the player root or its Lives owner)
        public abstract void Apply(GameObject collector);
    }
}