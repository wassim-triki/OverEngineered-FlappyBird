using UnityEngine;
using UnityEngine.UI;
public class LifeCell : MonoBehaviour
{
    [SerializeField] private GameObject full,empty;

    void SetFull()
    {
        if (full != null)
        {
            full.SetActive(true);
            empty.SetActive(false);
        }
    }

    void setEmpty()
    {
        if (empty != null) 
        {
            empty.SetActive(true);
            full.SetActive(false);
        }
    }

    public void SetImage(LifeCellStatus lifeCellStatus)
    {
        switch (lifeCellStatus)
        {
            case LifeCellStatus.Full:   
                SetFull();
                break;
            case LifeCellStatus.Empty:
                setEmpty();
                break;
            default:
                break;
        }
    }

    public enum LifeCellStatus 
    {
        Empty = 0,
        Full = 1,
    }
    
        
    [ContextMenu("Debug/SetFull")]
    private void Debug_SetFull() => SetImage(LifeCellStatus.Full);
    
    [ContextMenu("Debug/setEmpty")]
    private void Debug_SetEmpty() => SetImage(LifeCellStatus.Empty);
    
}
