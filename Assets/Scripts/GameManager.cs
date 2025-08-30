using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private BirdScript playerScript;
    [SerializeField] private PillarsSpawner pillarsSpawner;
    [SerializeField] private PillarsMover pillarsMover;
    
    void OnEnable()
    {
        playerLives.OnLifeLost += HandleLifeLost;    // ← subscribe
        playerLives.OnDeath    += HandleGameOver;
    }
    void OnDisable()
    {
        playerLives.OnLifeLost -= HandleLifeLost;
        playerLives.OnDeath    -= HandleGameOver;
    }
    void Start()
    {
        playerLives.ResetRun();
        playerScript.EnableControls();   
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

    void HandleLifeLost(DamageContext ctx)
    {
        if (ctx != null)
        {
            // if the ctx source is a top pillar or a bottom pillar than its parent must be a PillarPair
            var pair = ctx.Source.GetComponentInParent<PillarPair>();
            pair?.DisableScoring();                 
        }
    }
}
