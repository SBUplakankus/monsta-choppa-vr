using System;
using Databases;
using Pooling;
using Systems;
using UnityEngine;

namespace Visual_Effects
{
    /// <summary>
    /// Controls particle system playback with automatic return to pool.
    /// Implements <see cref="IUpdateable"/> for lifetime management.
    /// </summary>
    public class ParticleController : MonoBehaviour, IUpdateable
    {
        private ParticleSystem _ps;
        private float _endTime;
        
        /// <summary>
        /// Gets the particle configuration data.
        /// </summary>
        /// <value>The <see cref="ParticleData"/> containing effect settings.</value>
        public ParticleData Data { get; private set; }

        /// <summary>
        /// Initializes the controller with particle data.
        /// </summary>
        /// <param name="data">The <see cref="ParticleData"/> to configure this effect.</param>
        public void Initialise(ParticleData data)
        {
            Data = data;
        }

        /// <summary>
        /// Starts the particle system playback.
        /// </summary>
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

        /// <inheritdoc cref="IUpdateable.OnUpdate"/>
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