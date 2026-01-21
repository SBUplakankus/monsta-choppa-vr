using System;
using Events;
using UnityEngine;

namespace Systems.Core
{
    public class GameStateMessenger : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private GameStateEventChannel onGameStateChangeRequest;

        private void Start()
        {
            onGameStateChangeRequest.Raise(gameState);
        }
    }
}