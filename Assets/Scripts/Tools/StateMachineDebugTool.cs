using Events;
using Systems;
using UnityEngine;

namespace Tools
{
    public class StateMachineDebugTool : MonoBehaviour
    {
        [Header("State Machine Reference")]
        [SerializeField] private GameStateManager gameStateManager;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequested;

        private void Update()
        {
            // Input to trigger states for testing
            if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                PingState(GameState.GamePrelude);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                PingState(GameState.WaveIntermission);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                PingState(GameState.WaveActive);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                PingState(GameState.WaveComplete);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                PingState(GameState.BossIntermission);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                PingState(GameState.BossActive);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                PingState(GameState.BossComplete);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                PingState(GameState.GameWon);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                PingState(GameState.GameOver);
            }

            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                
            }
        }

        /// <summary>
        /// Pings a specific GameState to validate and trigger it.
        /// </summary>
        private void PingState(GameState state)
        {
            if (!gameStateManager)
            {
                Debug.LogError("StateMachineDebugTool: GameStateManager not assigned.");
                return;
            }

            Debug.Log($"StateMachineDebugTool: Pinging state {state}.");
            onGameStateChangeRequested?.Raise(state);
        }
    }
}