using System;
using Events;
using Events.Registries;
using UnityEngine;

namespace Systems.Core
{
    public class GameStateMessenger : MonoBehaviour
    {
        [SerializeField] private GameState gameState;

        private void Start()
        {
            GameplayEvents.GameStateChangeRequested.Raise(gameState);
        }
    }
}