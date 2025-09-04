using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameStateManager.OnMenu += HandleOnMenuState;
        GameStateManager.OnGameStarted += HandleGameStartedState;
        GameStateManager.OnGameResumed += HandleGameResumedState;
        GameStateManager.OnGamePaused += HandleGamePausedState;
        GameStateManager.OnGameOver += HandleOnGameOverState;
    }
    
    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnMenu -= HandleOnMenuState;
            GameStateManager.OnGameStarted -= HandleGameStartedState;
            GameStateManager.OnGameResumed -= HandleGameResumedState;
            GameStateManager.OnGamePaused -= HandleOnMenuState;
            GameStateManager.OnGameOver -= HandleOnGameOverState;
        }
    }
    
    
    void HandleOnGameOverState()
    {
        restartButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
    }
    

    void HandleGamePausedState()
    {
        resumeButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
    
    void HandleGameResumedState()
    {
        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
    }
    void HandleOnMenuState()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
    
    void HandleGameStartedState()
    {
        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
}
