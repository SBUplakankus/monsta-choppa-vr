using Events;
using Systems;
using Systems.Arena;
using UnityEngine;

namespace Tools
{
    public class StateMachineDebugTool : MonoBehaviour
    {
        [Header("State Machine Reference")]
        [SerializeField] private AreaStateManager areaStateManager;
        [SerializeField] private ArenaStateEventChannel onArenaStateChangeRequested;

        private void Update()
        {
            // Input to trigger states for testing
            if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                PingState(ArenaState.ArenaPrelude);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                PingState(ArenaState.WaveIntermission);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                PingState(ArenaState.WaveActive);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                PingState(ArenaState.WaveComplete);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                PingState(ArenaState.BossIntermission);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                PingState(ArenaState.BossActive);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                PingState(ArenaState.BossComplete);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                PingState(ArenaState.ArenaWon);
            }

            if (UnityEngine.InputSystem.Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                PingState(ArenaState.ArenaOver);
            }

            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                
            }
        }

        /// <summary>
        /// Pings a specific GameState to validate and trigger it.
        /// </summary>
        private void PingState(ArenaState state)
        {
            if (!areaStateManager)
            {
                Debug.LogError("StateMachineDebugTool: GameStateManager not assigned.");
                return;
            }

            Debug.Log($"StateMachineDebugTool: Pinging state {state}.");
            onArenaStateChangeRequested?.Raise(state);
        }
    }
}