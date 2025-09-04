using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameStateManager.OnMenu += HandleOnMenuState;
        GameStateManager.OnGameStarted += HandleGameStartedState;
        GameStateManager.OnGameResumed += HandleGameResumedState;
        GameStateManager.OnGamePaused += HandleGamePausedState;
    }
    
    void OnDisable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.OnMenu -= HandleOnMenuState;
            GameStateManager.OnGameStarted -= HandleGameStartedState;
            GameStateManager.OnGameResumed -= HandleGameResumedState;
            GameStateManager.OnGamePaused -= HandleOnMenuState;
        }
    }

    void HandleGamePausedState()
    {
        resumeButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
    
    void HandleGameResumedState()
    {
        playButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }
    void HandleOnMenuState()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
    }
    
    void HandleGameStartedState()
    {
        playButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }
}
