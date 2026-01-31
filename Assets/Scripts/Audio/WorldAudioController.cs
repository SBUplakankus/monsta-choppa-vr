using Data.Core;
using Pooling;
using Systems;
using Systems.Core;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class WorldAudioController : MonoBehaviour, IUpdateable
    {
        private AudioSource _audioSource;

        private float _endTime;
        private Transform _followTarget;
        private Vector3 _followOffset;

        private bool _initialized;

        public WorldAudioData Data { get; private set; }

        public void Initialise(WorldAudioData data)
        {
            Data = data;
            ApplyAudioSettings();
            _initialized = true;
        }

        public void PlayAtPosition(Vector3 position)
        {
            if (!_initialized) return;

            transform.position = position;
            _followTarget = null;
            PlayInternal();
        }

        public void PlayAttached(Transform target, Vector3 localOffset)
        {
            if (!_initialized) return;

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

        public void Stop()
        {
            if (!_initialized) return;

            _audioSource.Stop();
            _endTime = Time.time;
            ReturnToPool();
        }

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

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (!_initialized) return;
            if (!GameUpdateManager.Instance) return;

            GameUpdateManager.Instance.Register(this, UpdatePriority.Low);
        }

        private void OnDisable()
        {
            if (!_initialized) return;
            if (!GameUpdateManager.Instance) return;

            GameUpdateManager.Instance.Unregister(this);
        }

        private void OnDestroy()
        {
            if (!_initialized) return;
            if (!GameUpdateManager.Instance) return;

            GameUpdateManager.Instance.Unregister(this);
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_initialized) return;

            if (_followTarget)
                transform.position = _followTarget.TransformPoint(_followOffset);

            if (Time.time >= _endTime && !_audioSource.isPlaying)
                ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (GamePoolManager.Instance)
            {
                GamePoolManager.Instance.ReturnWorldAudioPrefab(this);
            }
            else
                gameObject.SetActive(false);
        }
    }
}
