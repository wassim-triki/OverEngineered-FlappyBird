using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Collectibles/Heal Collectible Effect", fileName = "HealCollectibleEffect")] 
    public sealed class HealCollectibleEffect : CollectibleEffect
    {
        [SerializeField, Min(1)] private int amount = 1;
        public override void Apply(GameObject collector)
        {
            if (!collector) return;
            var lives = collector.GetComponent<Lives>();
            if (!lives) return;

                lives.GainLife(amount);
        }
    }
}
