using UnityEngine;
using UnityEngine.UI;
public class LifeCell : MonoBehaviour
{
    [SerializeField] private GameObject full;

    public void SetFull(bool fill = true)
    {
        if (full != null)
            full.SetActive(fill);
    }
}
