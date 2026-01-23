using System;
using Events;
using UnityEngine;

namespace Systems.Core
{
    public class GameStateMessenger : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        private GameStateEventChannel _onGameStateChangeRequest;

        private void Awake()
        {
            _onGameStateChangeRequest = GameEvents.OnGameStateChangeRequested;
        }

        private void Start()
        {
            _onGameStateChangeRequest.Raise(gameState);
        }
    }
}