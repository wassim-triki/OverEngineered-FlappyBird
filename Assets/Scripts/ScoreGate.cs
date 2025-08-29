using UnityEngine;

public class ScoreGate : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private ScoreService _score;
    private bool _consumed;
    
    void OnEnable() => _consumed = false; // pool/reuse safe
    
    public void SetScoreService(ScoreService s) => _score = s;
    public void SetHeight(float gap)
    {
        transform.localScale = new Vector3(1,gap,1);
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
