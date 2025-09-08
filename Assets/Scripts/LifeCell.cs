using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LifeCell : MonoBehaviour
{
    [SerializeField] private Sprite icon; // filled heart
    private Image _image;
    private RectTransform _rt;
    private Vector3 _baseScale;
    private Tween _beatTween;

    void Awake()
    {
        _image = GetComponent<Image>();
        _rt    = GetComponent<RectTransform>();
        if (_image && icon) _image.sprite = icon;
        _baseScale = _rt ? _rt.localScale : Vector3.one;
    }

    /// <summary>
    /// Single beat. 'scale' is the absolute target multiplier (e.g. 1.15).
    /// Fixed timings/eases inside.
    /// </summary>
    public void Beat(float scale)
    {
        if (_rt == null) return;

        _beatTween?.Kill();
        _rt.localScale = _baseScale;

        _beatTween = DOTween.Sequence()
            .Append(_rt.DOScale(_baseScale * scale, 0.10f).SetEase(Ease.OutBack))
            .Append(_rt.DOScale(_baseScale,         0.18f).SetEase(Ease.OutCubic))
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}