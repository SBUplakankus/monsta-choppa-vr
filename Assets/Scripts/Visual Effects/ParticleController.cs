using System;
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
        
        public ParticleData Data { get; private set; }

        public void Initialise(ParticleData data)
        {
            Data = data;
        }

        public void Play()
        {
            _ps.Play(true);
            _endTime = Time.time + _ps.main.duration + _ps.main.startLifetime.constantMax;
        }

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable() => GameUpdateManager.Instance.Register(this, UpdatePriority.Low);
        private void OnDisable() => GameUpdateManager.Instance.Unregister(this);
        private void OnDestroy() => GameUpdateManager.Instance.Unregister(this);

        public void OnUpdate(float deltaTime)
        {
            if (Time.time < _endTime)
                return;
            
            if (!_ps.IsAlive(true))
            {
                GamePoolManager.Instance.ReturnParticlePrefab(this);
            }
        }
    }
}