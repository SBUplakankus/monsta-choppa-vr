using System;
using Events;
using UnityEngine;

namespace Systems.Core
{
    public class GameStateMessenger : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private GameStateEventChannel _onGameStateChangeRequest;

        private void Start()
        {
            _onGameStateChangeRequest.Raise(gameState);
        }
    }
}