using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Collectibles/Life Collectible Effect", fileName = "LifeCollectibleEffect")]
    public sealed class LifeCollectibleEffect : CollectibleEffect
    {
        [SerializeField, Min(1)] private int amount = 1;

        public override void Apply(GameObject collector)
        {
            if (!collector) return;
            var lives = collector.GetComponent<Lives>();
            if (!lives) return;
            lives.Max();
        }
    }
}