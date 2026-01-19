using Systems;
using UnityEngine;

namespace Data.Core
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/Core/Particles")]
    public class ParticleData : ScriptableObject
    {
        [SerializeField] private GameObject particlePrefab;
        [SerializeField] private VFXPriority vfxPriority =  VFXPriority.Low;
        
        public GameObject Prefab => particlePrefab;
        public string ID => name;
        public VFXPriority Priority => vfxPriority;
    }
}