using System;
using DefaultNamespace;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private GameObject highScoreUI;


    private void OnEnable()
    {
        GameStateManager.OnMenu += HandleOnMenu;
        GameStateManager.OnGameOver += HandleOnGameOver;
        GameStateManager.OnGamePaused += HandleOnGamePaused;
        GameStateManager.OnGameResumed += HandleOnGameResumed;
        GameStateManager.OnGameStarted += HandleOnGameStarted;
    }
    private void OnDisable()
    {
        GameStateManager.OnMenu -= HandleOnMenu;
        GameStateManager.OnGameOver -= HandleOnGameOver;
        GameStateManager.OnGamePaused -= HandleOnGamePaused;
        GameStateManager.OnGameResumed -= HandleOnGameResumed;
        GameStateManager.OnGameStarted -= HandleOnGameStarted;
    }


    void HandleOnGameStarted()
    {
        highScoreUI.SetActive(false);
    }
    void HandleOnGameResumed()
    {
        highScoreUI.SetActive(false);
    }
    
    void HandleOnMenu()
    {
        highScoreUI.gameObject.SetActive(true);
    }

    void HandleOnGameOver()
    {
        highScoreUI.gameObject.SetActive(true);
    }

    void HandleOnGamePaused()
    {
        highScoreUI.gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
