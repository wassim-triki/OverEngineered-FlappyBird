using System;
using DefaultNamespace;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private HighScoreUI highScoreUI;


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
        // highScoreUI.SetActive(false);
        highScoreUI.Hide();
    }
    
    void HandleOnGameResumed()
    {
        // highScoreUI.SetActive(false);
        highScoreUI.Hide();
    }
    
    
    void HandleOnMenu()
    {
        // highScoreUI.gameObject.SetActive(true);
        highScoreUI.Show();
    }

    void HandleOnGameOver()
    {
        // highScoreUI.gameObject.SetActive(true);
        highScoreUI.Show();
    }

    void HandleOnGamePaused()
    {
        // highScoreUI.gameObject.SetActive(true);
        highScoreUI.Show();
    }

}
