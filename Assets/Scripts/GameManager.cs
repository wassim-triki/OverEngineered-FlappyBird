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
    [SerializeField] private ScoreService score;
    [SerializeField] private CameraShake2D cameraShake;

    
    
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
        cameraShake?.ShakeHeavy();
        Haptics.Heavy();
        AudioManager.I.Play(Sfx.Death);
        GameStateManager.Instance.EndGame();
    }
    void InitializeGame()
    {
        // TODO: change this to Menu state
        GameStateManager.Instance.ReturnToMenu();
    }
    
    
    void HandleOnMenuState()
    {
        playerScript.ResetPlayer();
        playerScript.DisableMovements();
        playerLives.ResetLives();
        score.ResetScore();
        pipesSpawner.Disable();
        groundLoop.Unfreeze();
    }

    void HandleGameStartedState()
    {
        playerScript.EnableMovements();
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
        cameraShake?.ShakeLight(); 
        Haptics.Medium();
        AudioManager.I.Play(Sfx.Hit);
        if (ctx != null)
        {
            // if the ctx source is a top pillar or a bottom pillar than its parent must be a PillarPair
            var pair = ctx.Source.GetComponentInParent<PipePair>();
            pair?.DisableScoring();                 
        }
    }
}
