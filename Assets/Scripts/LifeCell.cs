using System;
using UnityEngine;
using UnityEngine.UI;
public class LifeCell : MonoBehaviour
{
    [SerializeField] private Sprite full;
    [SerializeField] private Sprite empty;
    private Image _image;

    public void SetFull(bool full = true)
    {
        if(_image == null) _image = GetComponent<Image>();
        if (full) _image.sprite = this.full;
        if (!full) _image.sprite = empty;
    }
}
