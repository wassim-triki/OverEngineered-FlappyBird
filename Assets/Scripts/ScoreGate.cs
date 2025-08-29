using UnityEngine;

public class ScoreGate : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private BoxCollider2D _collider;
    private ScoreService _score;
    private bool _consumed;
    
    void OnEnable() => _consumed = false; // pool/reuse safe
    
    public void SetScoreService(ScoreService s) => _score = s;
    public void SetHeight(float gap)
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
        _collider.size = new Vector2(_collider.size.x, gap); 
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (_consumed || _score == null) return;
        if (!other.CompareTag(playerTag)) return;

        _consumed = true;
        _score.Add(1);
    }
}
