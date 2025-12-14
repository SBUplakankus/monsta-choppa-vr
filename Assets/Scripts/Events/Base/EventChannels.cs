using UnityEngine;

namespace Events.Base
{
    public static class EventChannels
    {
        [CreateAssetMenu(fileName = "IntEventChannel", menuName = "Scriptable Objects/Event Channels/Int")]
        public class IntEventChannel : TypedEventChannel<int> { }

        [CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Scriptable Objects/Event Channels/Float")]
        public class FloatEventChannel : TypedEventChannel<float> { }
    
        [CreateAssetMenu(fileName = "StringEventChannel", menuName = "Scriptable Objects/Event Channels/String")]
        public class StringEventChannel : TypedEventChannel<string> { }

        [CreateAssetMenu(fileName = "V3EventChannel", menuName = "Scriptable Objects/Event Channels/Vector3")]
        public class Vector3EventChannel : TypedEventChannel<Vector3> { }
    }
}
