using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private Player playerScript;
    [SerializeField] private DifficultyController difficulty;
    [SerializeField] private PipesSpawner pipesSpawner;
    [SerializeField] private GroundLoop groundLoop;
    
    
    [SerializeField] private PipesMover pipesMover;
    
    void OnEnable()
    {
        playerLives.OnLifeLost += HandleLifeLost;    // ← subscribe
        playerLives.OnDeath    += HandleGameOver;
        GameStateManager.OnGameOver += HandleGameOverState;
        GameStateManager.OnGameStarted += HandleGameStartedState;
    }
    void OnDisable()
    {
        playerLives.OnLifeLost -= HandleLifeLost;
        playerLives.OnDeath -= HandleGameOver;
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnGameOver -= HandleGameOverState;
            GameStateManager.OnGameStarted -= HandleGameStartedState;
        }
    }
    void Start()
    {
        InitializeGame();
    }

    void HandleGameOver(DamageContext ctx)
    {
        GameStateManager.Instance.EndGame();
    }

    void InitializeGame()
    {
        GameStateManager.Instance.ReturnToMenu();
    }
    void HandleGameOverState()
    {
        playerScript.DisableControls();
        pipesSpawner.Disable();
        groundLoop.Freeze();
    }

    void HandleGameStartedState()
    {
        playerLives.ResetRun();
        pipesSpawner.Enable();
        playerScript.EnableControls();
        difficulty.ResetRun();
        groundLoop.Unfreeze();
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
