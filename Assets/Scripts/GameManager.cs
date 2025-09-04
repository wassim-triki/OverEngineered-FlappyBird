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
    
    
    void OnEnable()
    {
        playerLives.OnLifeLost += HandleLifeLost;    // ← subscribe
        playerLives.OnDeath    += HandleGameOver;
        GameStateManager.OnGameOver += HandleGameOverState;
        GameStateManager.OnGameStarted += HandleGameStartedState;
        GameStateManager.OnGamePaused += HandleGamePausedState;
        GameStateManager.OnGameResumed += HandleGameResumedState;
        GameStateManager.OnMenu += HandleOnMenuState;
    }
    void OnDisable()
    {
        playerLives.OnLifeLost -= HandleLifeLost;
        playerLives.OnDeath -= HandleGameOver;
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnGameOver -= HandleGameOverState;
            GameStateManager.OnGameStarted -= HandleGameStartedState;
            GameStateManager.OnGamePaused -= HandleGamePausedState;
            GameStateManager.OnGameResumed -= HandleGameResumedState;
            GameStateManager.OnMenu -= HandleOnMenuState;
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
        // TODO: change this to Menu state
        GameStateManager.Instance.ReturnToMenu();
    }
    
    
    void HandleOnMenuState()
    {
        playerScript.DisableMovements();
        pipesSpawner.Disable();
        groundLoop.Unfreeze();
    }

    void HandleGameStartedState()
    {
        playerScript.ResetAutoJump();
        playerScript.EnableMovements();
        playerLives.ResetLives();
        pipesSpawner.Enable();
        difficulty.ResetRun();
        groundLoop.Unfreeze();
    }

    void HandleGameOverState()
    {
        playerScript.DisableControls();
        pipesSpawner.Disable();
        groundLoop.Freeze();
    }
    void HandleGamePausedState()
    {
        playerScript.DisableMovements();
        pipesSpawner.Disable();
        groundLoop.Freeze();
    }
    
    void HandleGameResumedState()
    {
        playerScript.EnableMovements();
        pipesSpawner.Enable();
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
