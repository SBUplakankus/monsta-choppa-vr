using Systems.Arena;
using Systems.Core;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "GameStateEventChannel", menuName = "Scriptable Objects/Event Channels/Game State")]
    public class GameStateEventChannel : TypeEventChannelBase<GameState> { }
}