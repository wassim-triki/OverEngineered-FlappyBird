using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CollectibleEffect : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField] private Sfx sfxOnPickup = Sfx.Pickup;   // default

        public Sfx SfxOnPickup => sfxOnPickup;

        public abstract void Apply(GameObject collector);
    }
}