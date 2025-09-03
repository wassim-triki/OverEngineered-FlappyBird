using UnityEngine;

public class PipePair : MonoBehaviour
{
    [Header("Children")]
    [SerializeField] private Transform topAnchor;
    [SerializeField] private Transform bottomAnchor;
    [SerializeField] private ScoreGate scoreGate;
    [SerializeField] private PipesMover mover;

    [Header("Design")]
    [SerializeField]
    [Range(0f, 16f)]
    private float gap = 0f;
    
    [SerializeField] private float deadZone = -50;

    public float Gap => gap;

    void Update()
    {
        if (transform.position.x <= deadZone) Destroy(gameObject);
    }

    public void SetGap(float newGap)
    {
        gap = Mathf.Max(0f, newGap);
        float half = gap * 0.5f;
        if (topAnchor)    topAnchor.localPosition    = new Vector3(0f, +half, 0f);
        if (bottomAnchor) bottomAnchor.localPosition = new Vector3(0f, -half, 0f);
    }

    void OnValidate()
    {
        if (topAnchor && bottomAnchor) SetGap(gap);
    }

    public void Initialize(ScoreService score, DifficultyController difficulty)
    {
        if (scoreGate) scoreGate.SetScoreService(score);
        if (mover)     mover.Initialize(difficulty);
    }

    public void DisableScoring()
    {
        if (scoreGate) Destroy(scoreGate.gameObject);
    }
}