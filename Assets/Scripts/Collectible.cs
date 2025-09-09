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

        var player = other.GetComponentInParent<Player>();
        var collector = player ? player.gameObject : other;

        effect.Apply(collector);

        AudioManager.I?.Play(effect.SfxOnPickup);

        Haptics.Medium();

        if (destroyOnCollect) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}