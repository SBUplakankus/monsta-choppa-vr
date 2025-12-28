using Systems;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/World Audio")]
    public class WorldAudioData : ScriptableObject
    {
        #region Audio Properties
        
        [Header("Audio")]
        [SerializeField] private string audioId;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private GameObject audioPrefab;
        [SerializeField] private AudioPriority audioPriority;
        
        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private Vector2 randomVolumeRange = new (0.9f, 1.1f);
        
        [Header("Pitch Settings")]
        [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
        [SerializeField] private Vector2 randomPitchRange = new (0.95f, 1.05f);
        
        #endregion
        
        #region 3D Spatial Settings
        
        [Header("3D Spatial Settings")]
        [SerializeField, Range(0f, 1f)] private float spatialBlend = 1f;
        [SerializeField, Range(0f, 5f)] private float dopplerLevel = 1f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 50f;
        
        [Header("Rolloff Mode")]
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        
        #endregion
        
        #region Properties
        
        public string ID =>  audioId;
        public AudioClip AudioClip => audioClip;
        public GameObject Prefab => audioPrefab;
        public AudioPriority Priority => audioPriority;
        
        public float Volume => volume;
        public Vector2 RandomVolumeRange => randomVolumeRange;
        
        public float Pitch => pitch;
        public Vector2 RandomPitchRange => randomPitchRange;
        
        public float SpatialBlend => spatialBlend;
        public float DopplerLevel => dopplerLevel;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public AudioRolloffMode RolloffMode => rolloffMode;
        
        #endregion
        
        #region Methods
        
        public float GetRandomizedVolume()
        {
            return volume * UnityEngine.Random.Range(randomVolumeRange.x, randomVolumeRange.y);
        }
        
        public float GetRandomizedPitch()
        {
            return pitch * UnityEngine.Random.Range(randomPitchRange.x, randomPitchRange.y);
        }
        
        #endregion
    }
}
