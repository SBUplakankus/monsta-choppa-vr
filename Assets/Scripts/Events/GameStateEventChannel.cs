using Systems;
using Systems.Arena;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "GameStateEventChannel", menuName = "Scriptable Objects/Event Channels/Game State")]
    public class GameStateEventChannel : TypeEventChannelBase<GameState> { }
}