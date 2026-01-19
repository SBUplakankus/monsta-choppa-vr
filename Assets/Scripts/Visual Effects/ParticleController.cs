using Data.Core;
using Databases;
using Pooling;
using Systems;
using UnityEngine;

namespace Visual_Effects
{
    public class ParticleController : MonoBehaviour, IUpdateable
    {
        private ParticleSystem _ps;
        private float _endTime;

        private bool _initialized;

        public ParticleData Data { get; private set; }

        public void Initialise(ParticleData data)
        {
            Data = data;
            _initialized = true;
        }

        public void Play()
        {
            if (!_initialized) return;

            _ps.Play(true);
            _endTime = Time.time + _ps.main.duration + _ps.main.startLifetime.constantMax;
        }

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
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

            if (Time.time < _endTime)
                return;

            if (!_ps.IsAlive(true))
                GamePoolManager.Instance.ReturnParticlePrefab(this);
        }
    }
}