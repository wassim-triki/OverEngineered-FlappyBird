using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject gameOverMenu;
    
    
    
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
        gameOverMenu.SetActive(true);
        
        mainMenu.SetActive(false);
        inGameMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }
    

    void HandleGamePausedState()
    {
        pauseMenu.SetActive(true);
        
        mainMenu.SetActive(false);
        inGameMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
    
    void HandleGameResumedState()
    {
        inGameMenu.SetActive(true);
        
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);;
    }
    void HandleOnMenuState()
    {
        mainMenu.SetActive(true);
        
        pauseMenu.SetActive(false);
        inGameMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
    
    void HandleGameStartedState()
    {
        inGameMenu.SetActive(true);
        
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
}
