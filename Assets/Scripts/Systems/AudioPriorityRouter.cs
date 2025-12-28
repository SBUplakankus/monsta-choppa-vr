using UnityEngine;

namespace Systems
{
    public enum AudioPriority
    {
        Low,
        Medium,
        High
    };
    
    public class AudioPriorityRouter : MonoBehaviour
    {
        [Header("Global Audio Budget")]
        [Tooltip("Maximum number of active Audio allowed at once")]
        [SerializeField] private int maxActiveAudio = 100;

        [Tooltip("Lowest priority allowed when budget is exceeded")]
        [SerializeField] private AudioPriority maxPriorityWhenOverBudget = AudioPriority.High;

        private int _activeAudioCount;

        /// <summary>
        /// Returns true if Audio of the given priority is allowed to spawn.
        /// </summary>
        public bool CanSpawn(AudioPriority priority)
        {
            if (_activeAudioCount < maxActiveAudio)
                return true;

            // Budget exceeded → only allow important effects
            return priority <= maxPriorityWhenOverBudget;
        }

        /// <summary>
        /// Call when Audio successfully spawns.
        /// </summary>
        public void RegisterSpawn()
        {
            _activeAudioCount++;
        }

        /// <summary>
        /// Call when Audio is returned to the pool.
        /// </summary>
        public void RegisterDespawn()
        {
            _activeAudioCount--;
            if (_activeAudioCount < 0)
                _activeAudioCount = 0;
        }

#if UNITY_EDITOR
        public int ActiveAudioCount => _activeAudioCount;
#endif
    }
}