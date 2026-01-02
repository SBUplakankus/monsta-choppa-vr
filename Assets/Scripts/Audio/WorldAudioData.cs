using Systems;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// ScriptableObject containing configuration data for world audio playback.
    /// Includes spatial settings and randomization parameters.
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Data/World Audio")]
    public class WorldAudioData : ScriptableObject
    {
        #region Audio Properties
        
        [Header("Audio")]
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
        
        /// <summary>
        /// Gets the unique identifier for this audio data.
        /// </summary>
        /// <value>The name of the ScriptableObject.</value>
        public string ID =>  name;
        
        /// <summary>
        /// Gets the audio clip to play.
        /// </summary>
        /// <value>The <see cref="AudioClip"/> asset.</value>
        public AudioClip AudioClip => audioClip;
        
        /// <summary>
        /// Gets the prefab containing the <see cref="WorldAudioController"/>.
        /// </summary>
        /// <value>The GameObject prefab.</value>
        public GameObject Prefab => audioPrefab;
        
        /// <summary>
        /// Gets the priority level for audio spawning.
        /// </summary>
        /// <value>The <see cref="AudioPriority"/> enumeration value.</value>
        public AudioPriority Priority => audioPriority;
        
        /// <summary>
        /// Gets the base volume level before randomization.
        /// </summary>
        /// <value>Volume between 0.0 and 1.0.</value>
        public float Volume => volume;
        
        /// <summary>
        /// Gets the volume randomization range.
        /// </summary>
        /// <value>The <see cref="Vector2"/> representing min/max multipliers.</value>
        public Vector2 RandomVolumeRange => randomVolumeRange;
        
        /// <summary>
        /// Gets the base pitch value before randomization.
        /// </summary>
        /// <value>Pitch between -3.0 and 3.0.</value>
        public float Pitch => pitch;
        
        /// <summary>
        /// Gets the pitch randomization range.
        /// </summary>
        /// <value>The <see cref="Vector2"/> representing min/max multipliers.</value>
        public Vector2 RandomPitchRange => randomPitchRange;
        
        /// <summary>
        /// Gets the spatial blend setting (2D to 3D).
        /// </summary>
        /// <value>0.0 for 2D, 1.0 for 3D.</value>
        public float SpatialBlend => spatialBlend;
        
        /// <summary>
        /// Gets the Doppler effect intensity.
        /// </summary>
        /// <value>Doppler level between 0.0 and 5.0.</value>
        public float DopplerLevel => dopplerLevel;
        
        /// <summary>
        /// Gets the minimum distance for volume rolloff.
        /// </summary>
        /// <value>Distance in world units.</value>
        public float MinDistance => minDistance;
        
        /// <summary>
        /// Gets the maximum distance for hearing the audio.
        /// </summary>
        /// <value>Distance in world units.</value>
        public float MaxDistance => maxDistance;
        
        /// <summary>
        /// Gets the volume rolloff mode.
        /// </summary>
        /// <value>The <see cref="AudioRolloffMode"/> enumeration value.</value>
        public AudioRolloffMode RolloffMode => rolloffMode;
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Calculates a randomized volume using the base volume and random range.
        /// </summary>
        /// <returns>Randomized volume value.</returns>
        public float GetRandomizedVolume()
        {
            return volume * UnityEngine.Random.Range(randomVolumeRange.x, randomVolumeRange.y);
        }
        
        /// <summary>
        /// Calculates a randomized pitch using the base pitch and random range.
        /// </summary>
        /// <returns>Randomized pitch value.</returns>
        public float GetRandomizedPitch()
        {
            return pitch * UnityEngine.Random.Range(randomPitchRange.x, randomPitchRange.y);
        }
        
        #endregion
    }
}