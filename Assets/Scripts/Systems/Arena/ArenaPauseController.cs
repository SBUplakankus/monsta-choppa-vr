using Events;
using Systems.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Arena
{
    public class ArenaPauseController : MonoBehaviour
    {
        #region Fields

        [Header("Events")] 
        [SerializeField] private VoidEventChannel onPause;
        [SerializeField] private VoidEventChannel onResumed;
        [SerializeField] private ArenaStateEventChannel onArenaStateChangeRequested;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionReference pauseAction;
        private bool _isPaused;

        #endregion
        
        #region Methods
        
        private void OnPauseButtonPressed(InputAction.CallbackContext context) => TogglePause();

        private void Unpause()
        {
            _isPaused = false;
            onResumed?.Raise();
            onGameStateChangeRequested?.Raise(GameState.Arena);
        }

        private void Pause()
        {
            _isPaused = true;
            onPause?.Raise();
            onArenaStateChangeRequested?.Raise(ArenaState.ArenaPaused); 
            onGameStateChangeRequested?.Raise(GameState.ArenaPaused);
        }

        private void TogglePause()
        {
            if (_isPaused)
                Unpause();   
            else
                Pause();
        }

        private void OnEnable()
        {
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.performed += OnPauseButtonPressed;
            }
            else
            {
                Debug.LogWarning("Pause Action is not assigned!");
            }
        }

        private void OnDisable()
        {
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.performed -= OnPauseButtonPressed;
            }
        }
        
        #endregion
    }
}