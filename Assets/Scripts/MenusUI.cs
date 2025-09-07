using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject gameOverMenu;
    
    private RectTransform _mainRectTransform;
    
    
    
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

    private void Start()
    {
        _mainRectTransform = mainMenu.GetComponent<RectTransform>();
        // Initialize to main menu state
        HandleOnMenuState();
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
        _mainRectTransform.DOAnchorPosY(-1200f, 0.5f).SetEase(Ease.OutBack).From();
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
