using DefaultNamespace;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Effect")] 
    [SerializeField] private CollectibleEffect effect;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyOnCollect = true;

    private bool _consumed;

    void OnEnable() => _consumed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed || effect == null) return;
        if (!other.CompareTag(playerTag)) return;
        Consume(other.gameObject);
    }

    private void Consume(GameObject other)
    {
        _consumed = true;
        GameObject collector = other;
        // Try to find a Player root in hierarchy to ensure we apply to the right object (Lives lives on same root as PlayerDamageable)
        var player = other.GetComponentInParent<Player>();
        if (player) collector = player.gameObject;
        effect.Apply(collector);
        AudioManager.I.Play(Sfx.Pickup);
        Haptics.Medium();
        if (destroyOnCollect) Destroy(gameObject); else gameObject.SetActive(false);
    }
}
