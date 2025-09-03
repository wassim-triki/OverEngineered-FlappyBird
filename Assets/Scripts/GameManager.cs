using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private Player playerScript;
    [SerializeField] private PipesSpawner pipesSpawner;
    [SerializeField] private PipesMover pipesMover;
    
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

        var diff = FindFirstObjectByType<DifficultyController>();
        if (diff) diff.ResetRun();
    }

    void HandleGameOver(DamageContext ctx)
    {
        playerScript.DisableControls();
        pipesSpawner.Freeze();
        foreach (PipesMover mover in FindObjectsByType<PipesMover>(FindObjectsSortMode.None))
        {
            mover.Freeze();
        }
    }

    void HandleLifeLost(DamageContext ctx)
    {
        if (ctx != null)
        {
            // if the ctx source is a top pillar or a bottom pillar than its parent must be a PillarPair
            var pair = ctx.Source.GetComponentInParent<PipePair>();
            pair?.DisableScoring();                 
        }
    }
}
