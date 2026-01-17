using Characters.Base;
using Pooling;
using UnityEngine;

namespace Weapons.Projectiles
{
    /// <summary>
    /// Projectile controller for arrows and other ranged weapon projectiles.
    /// VR-optimized with object pooling support and minimal per-frame allocations.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider hitCollider;
        [SerializeField] private TrailRenderer trailRenderer;

        private ProjectileData _data;
        private float _spawnTime;
        private float _damageMultiplier = 1f;
        private bool _hasHit;
        private Vector3 _velocity;

        // Cached for VR performance - avoid GetComponent calls
        private Transform _transform;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the projectile data configuration.
        /// </summary>
        public ProjectileData Data => _data;

        #endregion

        #region Initialization

        private void Awake()
        {
            // Cache transform reference for VR performance
            _transform = transform;
            
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!hitCollider) hitCollider = GetComponent<Collider>();
        }

        /// <summary>
        /// Initializes the projectile with data. Called by pool or spawner.
        /// </summary>
        /// <param name="data">Projectile configuration data.</param>
        public void Initialize(ProjectileData data)
        {
            _data = data;
        }

        /// <summary>
        /// Launches the projectile from the pool.
        /// </summary>
        /// <param name="position">Spawn position.</param>
        /// <param name="direction">Launch direction (normalized).</param>
        /// <param name="damageMultiplier">Damage multiplier from bow draw strength.</param>
        public void Launch(Vector3 position, Vector3 direction, float damageMultiplier = 1f)
        {
            _transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
            _damageMultiplier = damageMultiplier;
            _hasHit = false;
            _spawnTime = Time.time;

            // Set initial velocity
            _velocity = direction.normalized * _data.Speed * damageMultiplier;
            
            // Configure rigidbody for flight
            rb.isKinematic = false;
            rb.linearVelocity = _velocity;
            rb.useGravity = false; // We handle gravity manually for better control
            
            // Enable collision
            hitCollider.enabled = true;

            // Reset trail
            if (trailRenderer)
            {
                trailRenderer.Clear();
                trailRenderer.enabled = true;
            }

            gameObject.SetActive(true);
        }

        #endregion

        #region Update

        private void FixedUpdate()
        {
            if (_hasHit) return;

            // Check lifetime
            if (Time.time - _spawnTime >= _data.Lifetime)
            {
                ReturnToPool();
                return;
            }

            // Apply gravity (VR-friendly arc trajectory)
            if (_data.GravityMultiplier > 0)
            {
                _velocity += Physics.gravity * (_data.GravityMultiplier * Time.fixedDeltaTime);
                rb.linearVelocity = _velocity;
            }

            // Rotate to face velocity direction for realistic arrow flight
            if (_velocity.sqrMagnitude > 0.01f)
            {
                _transform.rotation = Quaternion.LookRotation(_velocity.normalized);
            }
        }

        #endregion

        #region Collision

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHit) return;

            // Ignore other projectiles
            if (other.GetComponent<Projectile>()) return;

            ProcessHit(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHit) return;

            // Ignore other projectiles
            if (collision.collider.GetComponent<Projectile>()) return;

            ProcessHit(collision.collider);
        }

        /// <summary>
        /// Processes a hit on a target or surface.
        /// </summary>
        private void ProcessHit(Collider other)
        {
            _hasHit = true;

            var hitPoint = _transform.position;
            var hitRotation = _transform.rotation;

            // Check for damageable target
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                var finalDamage = Mathf.RoundToInt(_data.BaseDamage * _damageMultiplier);
                finalDamage = Mathf.Max(1, finalDamage);
                target.TakeDamage(finalDamage);
            }

            // Spawn hit effects
            SpawnHitEffects(hitPoint, hitRotation);

            // Stop movement
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            hitCollider.enabled = false;

            // Disable trail
            if (trailRenderer)
                trailRenderer.enabled = false;

            // Return to pool after brief delay (so effects can play)
            Invoke(nameof(ReturnToPool), 0.1f);
        }

        /// <summary>
        /// Spawns hit VFX and audio using the pool manager.
        /// </summary>
        private void SpawnHitEffects(Vector3 position, Quaternion rotation)
        {
            var poolManager = GamePoolManager.Instance;
            if (poolManager == null) return;

            if (_data.HitVFX != null)
                poolManager.GetParticlePrefab(_data.HitVFX, position, rotation);

            if (_data.HitSfx != null)
                poolManager.GetWorldAudioPrefab(_data.HitSfx, position);
        }

        #endregion

        #region Pooling

        /// <summary>
        /// Returns this projectile to the pool.
        /// </summary>
        private void ReturnToPool()
        {
            CancelInvoke();
            _hasHit = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            hitCollider.enabled = false;
            
            if (trailRenderer)
            {
                trailRenderer.Clear();
                trailRenderer.enabled = false;
            }

            gameObject.SetActive(false);
            
            // Note: Pool manager should handle returning to pool
            // For now, we just deactivate. Integration with GamePoolManager
            // can be added when ProjectileDatabase is implemented.
        }

        /// <summary>
        /// Called when returned to pool. Resets state.
        /// </summary>
        public void OnDespawn()
        {
            ReturnToPool();
        }

        #endregion
    }
}
