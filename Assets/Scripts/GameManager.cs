using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private BirdScript playerScript;
    [SerializeField] private PillarsSpawner pillarsSpawner;
    [SerializeField] private PillarsMover pillarsMover;
    
    void OnEnable()  => playerLives.OnDeath += HandleGameOver;
    void OnDisable() => playerLives.OnDeath -= HandleGameOver;
    void Start()
    {
        playerLives.ResetRun();
    }

    void HandleGameOver(DamageContext ctx)
    {
        playerScript.DisableControls();
        pillarsSpawner.Freeze();
        foreach (PillarsMover mover in FindObjectsByType<PillarsMover>(FindObjectsSortMode.None))
        {
            mover.Freeze();
        }
    }
}
