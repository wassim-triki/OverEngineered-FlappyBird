// Assets/Scripts/SlomoCollectibleEffect.cs
using UnityEngine;
using DG.Tweening;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Collectibles/Slomo Collectible Effect", fileName = "SlomoCollectibleEffect")]
    public sealed class SlomoCollectibleEffect : CollectibleEffect
    {
        [Header("Pulse Settings")]
        [Range(0.1f, 1f)] [SerializeField] private float targetMultiplier = 0.55f;
        [SerializeField] private float inDuration   = 0.22f;
        [SerializeField] private float holdDuration = 3.00f;   // << NEW (default 3s)
        [SerializeField] private float outDuration  = 0.90f;
        [SerializeField] private Ease  inEase       = Ease.OutCubic;
        [SerializeField] private Ease  outEase      = Ease.InCubic;

        public override void Apply(GameObject collector)
        {
            if (!collector) return;

            // Gameplay speed pulse
            var slomo = Object.FindFirstObjectByType<SlomoController>();
            if (slomo != null)
                slomo.PlayPulse(targetMultiplier, inDuration, holdDuration, outDuration, inEase, outEase);

            // Music pulse (underwater-esque)
            var audio = AudioManager.I;
            if (audio != null)
                audio.PulseUnderwater(inDuration, holdDuration, outDuration, inEase, outEase);

            // Post-processing pulse (lens + chroma only; NO vignette change)
            var post = Object.FindFirstObjectByType<PostProcessing>();
            if (post != null)
            {
                post.PulseUnderwaterVisuals(inDuration, holdDuration, outDuration, inEase, outEase);
                post.PulseCamZoom(inDuration, holdDuration, outDuration, inEase, outEase);
            }
                
        }
    }
}