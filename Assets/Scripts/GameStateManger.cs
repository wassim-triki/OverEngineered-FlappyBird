using System;
using System.Drawing;
using UnityEngine;

namespace DefaultNamespace
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }
    public class GameStateManger:MonoBehaviour
    {
        public static GameStateManger Instance { get; private set; }
        // TODO: change this to Menu state
        [SerializeField] private GameState currentState = GameState.Playing;
        public GameState CurrentState => currentState;

        public static event Action<GameState,GameState> OnStateChanged;
        public static event Action OnGameOver;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameStarted;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetState(GameState newState)
        {
            if (newState == currentState) return;
            var oldState = currentState;
            currentState = newState;
            OnStateChanged?.Invoke(oldState, newState);
            switch (newState)
            {
                case GameState.Menu:
                    break;
                case GameState.Playing:
                    if (oldState == GameState.Paused)
                    {
                        OnGameResumed?.Invoke();
                    }

                    if (oldState == GameState.GameOver || oldState == GameState.Menu)
                    {
                        OnGameStarted?.Invoke();
                    }

                    break;
                case GameState.Paused:
                    OnGamePaused?.Invoke();
                    break;
                case GameState.GameOver:
                    OnGameOver?.Invoke();
                    break;
            }

            Debug.Log($"[GameState] {oldState} → {newState}");
        }

        public void ReturnToMenu() => SetState(GameState.Menu);
        public void StartGame() => SetState(GameState.Playing);
        public void PauseGame() => SetState(GameState.Paused);
        public void ResumeGame() => SetState(GameState.Playing);
        public void GameOver() => SetState(GameState.GameOver);
    }
}