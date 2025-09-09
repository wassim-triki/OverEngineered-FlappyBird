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
    [SerializeField] private DamageSource damageSource;
    
    
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
        GameStateManager.Instance.ReturnToMenu();
    }
    
    
    void HandleOnMenuState()
    {
        playerScript.ResetPlayer();
        playerScript.DisableMovements();
        playerScript.AnimateMenuAppear(); 
        playerLives.ResetLives();
        score.ResetScore();
        pipesSpawner.Disable();
        groundLoop.Unfreeze();
        damageSource.ResetCollider();
    }

    void HandleGameStartedState()
    {
        playerScript.EnsureNormalScale();
        playerScript.EnableMovements();
        playerScript.StartSnapX(); 
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
            var pair = ctx.Source.GetComponentInParent<PipePair>();
            pair?.DisableScoring();                 
        }
    }
}
