using DefaultNamespace;
using UnityEngine;

public class ScoreGate : MonoBehaviour
{
    private string _playerTag = "Player";
    private ScoreService _score;
    private bool _consumed;
    
    void OnEnable() => _consumed = false; // pool/reuse safe
    
    public void SetScoreService(ScoreService s) => _score = s;

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed || _score == null) return;
        if (!other.CompareTag(_playerTag)) return;

        _consumed = true;
        _score.Add(1);
        AudioManager.I.Play(Sfx.Score);
        Haptics.Light();
    }
}
