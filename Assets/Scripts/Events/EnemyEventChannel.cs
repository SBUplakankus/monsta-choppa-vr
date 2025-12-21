using Characters.Enemies;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Scriptable Objects/Event Channels/Enemy")]
    public class EnemyEventChannel : TypeEventChannelBase<EnemyController> {}
}