using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Lives PlayerLives;
    
    void Start()
    {
        PlayerLives.ResetRun();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
