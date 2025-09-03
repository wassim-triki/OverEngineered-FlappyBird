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
    public class GameStateManager:MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        
        [SerializeField] private GameState currentState = GameState.Paused;
        private GameState _previousState;
        public GameState CurrentState => currentState;

        public static event Action<GameState,GameState> OnStateChanged;
        public static event Action OnGameOver;
        public static event Action OnGamePaused;
        public static event Action OnGamePlaying;
        public static event Action OnMenu;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _previousState = currentState;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (_previousState != currentState)
            {
                SetState(currentState);
                _previousState = currentState;
                
            }
#endif
        }

        public void SetState(GameState newState)
        {
            // if (newState == currentState && _previousState == currentState) return;
            
            var oldState = _previousState != currentState ? _previousState : currentState;
            currentState = newState;
            _previousState = newState;
            
            OnStateChanged?.Invoke(oldState, newState);
            
            switch (newState)
            {
                case GameState.Menu:
                    OnMenu?.Invoke();
                    break;
                case GameState.Playing:
                    OnGamePlaying?.Invoke();
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
        public void EndGame() => SetState(GameState.GameOver);
        

    }
    
}