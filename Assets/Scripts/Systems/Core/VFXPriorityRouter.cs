using UnityEngine;

namespace Systems
{
    public enum VFXPriority { Low, Medium, High, Critical };
    
    public class VFXPriorityRouter : MonoBehaviour
    {
        [Header("Global VFX Budget")]
        [Tooltip("Maximum number of active VFX allowed at once")]
        [SerializeField] private int maxActiveVfx = 100;

        [Tooltip("Lowest priority allowed when budget is exceeded")]
        [SerializeField] private VFXPriority maxPriorityWhenOverBudget = VFXPriority.High;

        private int _activeVfxCount;

        /// <summary>
        /// Returns true if a VFX of the given priority is allowed to spawn.
        /// </summary>
        public bool CanSpawn(VFXPriority priority)
        {
            if (_activeVfxCount < maxActiveVfx)
                return true;

            // Budget exceeded → only allow important effects
            return priority <= maxPriorityWhenOverBudget;
        }

        /// <summary>
        /// Call when a VFX successfully spawns.
        /// </summary>
        public void RegisterSpawn()
        {
            _activeVfxCount++;
        }

        /// <summary>
        /// Call when a VFX is returned to the pool.
        /// </summary>
        public void RegisterDespawn()
        {
            _activeVfxCount--;
            if (_activeVfxCount < 0)
                _activeVfxCount = 0;
        }

#if UNITY_EDITOR
        public int ActiveVfxCount => _activeVfxCount;
#endif
    }
}