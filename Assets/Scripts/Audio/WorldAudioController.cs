using Pooling;
using Systems;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Controls spatial audio playback in the game world with support for following transforms.
    /// Implements <see cref="IUpdateable"/> for frame-by-frame updates.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class WorldAudioController : MonoBehaviour, IUpdateable
    {
        #region Components
        
        private AudioSource _audioSource;
        
        #endregion
        
        #region Runtime Variables
        
        private float _endTime;
        private Transform _followTarget;
        private Vector3 _followOffset;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the audio configuration data for this controller.
        /// </summary>
        /// <value>The <see cref="WorldAudioData"/> containing audio settings.</value>
        public WorldAudioData Data { get; private set; }
        
        #endregion
        
        #region Initialisation
        
        /// <summary>
        /// Initializes the controller with audio configuration data.
        /// </summary>
        /// <param name="data">The <see cref="WorldAudioData"/> containing audio settings.</param>
        public void Initialise(WorldAudioData data)
        {
            Data = data;
            ApplyAudioSettings();
        }
        
        #endregion
        
        #region Playback Methods
        
        /// <summary>
        /// Plays the audio at a fixed world position.
        /// </summary>
        /// <param name="position">World position to play the audio.</param>
        public void PlayAtPosition(Vector3 position)
        {
            transform.position = position;
            _followTarget = null;
            PlayInternal();
        }

        /// <summary>
        /// Plays the audio attached to a transform with local offset.
        /// </summary>
        /// <param name="target">Transform to follow during playback.</param>
        /// <param name="localOffset">Local offset relative to the target transform.</param>
        public void PlayAttached(Transform target, Vector3 localOffset)
        {
            _followTarget = target;
            _followOffset = localOffset;
            transform.position = target.TransformPoint(localOffset);
            PlayInternal();
        }

        private void PlayInternal()
        {
            if (_audioSource.clip == null) return;
            _audioSource.Play();
            _endTime = Time.time + _audioSource.clip.length + 0.1f;
        }

        /// <summary>
        /// Stops audio playback and returns the object to the pool.
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
            _endTime = Time.time;
            ReturnToPool();
        }
        
        #endregion
        
        #region Audio Configuration
        
        private void ApplyAudioSettings()
        {
            _audioSource.clip = Data.AudioClip;
            _audioSource.volume = Data.GetRandomizedVolume();
            _audioSource.pitch = Data.GetRandomizedPitch();
            _audioSource.spatialBlend = Data.SpatialBlend;
            _audioSource.dopplerLevel = Data.DopplerLevel;
            _audioSource.minDistance = Data.MinDistance;
            _audioSource.maxDistance = Data.MaxDistance;
            _audioSource.rolloffMode = Data.RolloffMode;
            _audioSource.loop = false;
        }
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable() => GameUpdateManager.Instance.Register(this, UpdatePriority.Low);
        private void OnDisable() => GameUpdateManager.Instance.Unregister(this);
        private void OnDestroy() => GameUpdateManager.Instance.Unregister(this);
        
        #endregion
        
        #region IUpdateable Implementation
        
        /// <inheritdoc cref="IUpdateable.OnUpdate"/>
        public void OnUpdate(float deltaTime)
        {
            if (_followTarget)
            {
                transform.position = _followTarget.TransformPoint(_followOffset);
            }

            if (Time.time >= _endTime && !_audioSource.isPlaying)
            {
                ReturnToPool();
            }
        }
        
        #endregion
        
        #region Pool Management
        
        private void ReturnToPool()
        {
            if (GamePoolManager.Instance)
            {
                // GamePoolManager.Instance.ReturnAudioPrefab(this);
            }
            else
                gameObject.SetActive(false);
        }
        
        #endregion
    }
}