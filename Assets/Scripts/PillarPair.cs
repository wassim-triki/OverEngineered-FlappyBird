using System;
using UnityEngine;

public class PillarPair : MonoBehaviour
{
    [Header("Children")]
    [SerializeField] private Transform topAnchor;
    [SerializeField] private Transform bottomAnchor;
    [SerializeField] private ScoreGate scoreGate;
    
    [Header("Design")]
    [SerializeField, Min(0f)] private float gap = 0f;
    [SerializeField] private float deadZone = -50;
    
    public float Gap => gap;

    private void Update()
    {
        bool reachedDeadZone = transform.position.x <= deadZone;
        if(reachedDeadZone) Destroy(gameObject);
    }

    public void SetGap(float newGap)
    {
        gap = Mathf.Max(0f, newGap);
        float half = gap * 0.5f;
        if (topAnchor)    topAnchor.localPosition    = new Vector3(0f, +half, 0f);
        if (bottomAnchor) bottomAnchor.localPosition = new Vector3(0f, -half, 0f);
        if (scoreGate) scoreGate.SetHeight(gap);
    }
    
    private void OnValidate()
    {
        if (topAnchor && bottomAnchor) SetGap(gap); // preview the gap instantly in the editor
    }

    public void Initialize(ScoreService score)     // ← called by spawner
    {
        if (scoreGate) scoreGate.SetScoreService(score);
    }
}
