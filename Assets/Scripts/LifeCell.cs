using UnityEngine;
using UnityEngine.UI;

public class LifeCell : MonoBehaviour
{
    private Image _image;

    void Awake()
    {
        if (_image == null) _image = GetComponent<Image>();
    }


}