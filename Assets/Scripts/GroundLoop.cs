using System;
using UnityEngine;

public class GroundLoop : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private float _fallbackSpeed = 1f;
    private float _maxWidth;
    private Vector2 _startSize;
    [SerializeField] private DifficultyController difficulty;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startSize = new Vector2(_spriteRenderer.size.x, _spriteRenderer.size.y);
        _maxWidth = _startSize.x * 2;
    }

    private void Update()
    {
        float speed = difficulty? difficulty.CurrentSpeed : _fallbackSpeed;
        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x + speed * Time.deltaTime, _spriteRenderer.size.y);
        if (_spriteRenderer.size.x >= _maxWidth)
        {
            _spriteRenderer.size = _startSize;
        }
    }
}
