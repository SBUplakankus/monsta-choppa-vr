using Systems;
using Systems.Arena;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "ArenaStateEventChannel", menuName = "Scriptable Objects/Event Channels/Arena State")]
    public class ArenaStateEventChannel : TypeEventChannelBase<ArenaState> { }
}