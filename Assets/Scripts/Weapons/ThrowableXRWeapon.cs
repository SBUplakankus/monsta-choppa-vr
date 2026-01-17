using Characters.Base;
using Constants;
using Pooling;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Throwing weapon state for tracking flight and recall.
    /// </summary>
    public enum ThrowableState
    {
        Held,
        InFlight,
        Returning,
        Stuck
    }

    /// <summary>
    /// Throwable weapon (knives, axes) with boomerang recall mechanic.
    /// Player can hold trigger to recall the weapon to their hand.
    /// VR-optimized with physics-based flight.
    /// Works for both player and enemy use.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableXRWeapon : XRWeaponBase
    {
        #region Fields

        [Header("Throw Settings")]
        [SerializeField] private float throwForceMultiplier = 2f;
        [SerializeField] private float minThrowVelocity = 2f;
        [SerializeField] private float maxThrowVelocity = 15f;
        [SerializeField] private float spinSpeed = 720f;
        
        [Header("Return Settings")]
        [SerializeField] private float returnSpeed = 15f;
        [SerializeField] private float returnAcceleration = 20f;
        [SerializeField] private float catchDistance = 0.3f;
        [SerializeField] private float maxFlightTime = 5f;
        
        [Header("Damage Settings")]
        [SerializeField] private bool damageOnReturn = true;
        [SerializeField] private float returnDamageMultiplier = 0.5f;
        
        [Header("Stuck Settings")]
        [SerializeField] private float stickDuration = 3f;
        [SerializeField] private LayerMask stickableLayers;

        private Rigidbody _rb;
        private Collider _collider;
        private Transform _transform;
        private Transform _returnTarget;
        
        private ThrowableState _state = ThrowableState.Held;
        private Vector3 _throwVelocity;
        private float _flightStartTime;
        private float _stuckTime;
        private float _currentReturnSpeed;
        
        // Track last holder for return
        private Transform _lastHolder;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current state of the throwable weapon.
        /// </summary>
        public ThrowableState State => _state;

        /// <summary>
        /// Returns true if the weapon is currently in flight or returning.
        /// </summary>
        public bool IsFlying => _state == ThrowableState.InFlight || _state == ThrowableState.Returning;

        /// <summary>
        /// Returns true if the weapon can be recalled.
        /// </summary>
        public bool CanRecall => _state == ThrowableState.InFlight || _state == ThrowableState.Stuck;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _transform = transform;
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case ThrowableState.InFlight:
                    UpdateFlight();
                    break;
                case ThrowableState.Returning:
                    UpdateReturn();
                    break;
                case ThrowableState.Stuck:
                    UpdateStuck();
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_state == ThrowableState.InFlight)
            {
                HandleFlightCollision(collision);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_state == ThrowableState.InFlight || (_state == ThrowableState.Returning && damageOnReturn))
            {
                HandleTriggerHit(other);
            }
        }

        #endregion

        #region Throw

        /// <summary>
        /// Throws the weapon with the given velocity.
        /// </summary>
        /// <param name="velocity">Initial throw velocity.</param>
        public void Throw(Vector3 velocity)
        {
            var speed = velocity.magnitude;
            if (speed < minThrowVelocity) return;

            _throwVelocity = Vector3.ClampMagnitude(velocity * throwForceMultiplier, maxThrowVelocity);
            
            _state = ThrowableState.InFlight;
            _flightStartTime = Time.time;
            _currentReturnSpeed = 0f;
            
            // Detach from hand
            _transform.SetParent(null);
            
            // Apply physics
            _rb.isKinematic = false;
            _rb.linearVelocity = _throwVelocity;
            _rb.angularVelocity = _transform.right * spinSpeed * Mathf.Deg2Rad;
            
            // Enable collision
            if (_collider) _collider.isTrigger = false;
        }

        #endregion

        #region Flight

        /// <summary>
        /// Updates weapon during flight.
        /// </summary>
        private void UpdateFlight()
        {
            // Check max flight time
            if (Time.time - _flightStartTime > maxFlightTime)
            {
                StartReturn();
                return;
            }

            // Rotate to face velocity
            if (_rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                // Spin around forward axis
                _transform.Rotate(Vector3.right, spinSpeed * Time.fixedDeltaTime, Space.Self);
            }
        }

        /// <summary>
        /// Handles collision during flight.
        /// </summary>
        private void HandleFlightCollision(Collision collision)
        {
            // Check if we should stick
            if (((1 << collision.gameObject.layer) & stickableLayers) != 0)
            {
                StickToSurface(collision);
            }
            else
            {
                // Bounce off and start return
                StartReturn();
            }
        }

        /// <summary>
        /// Handles trigger hits during flight (damage enemies).
        /// </summary>
        private void HandleTriggerHit(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                var multiplier = _state == ThrowableState.Returning ? returnDamageMultiplier : 1f;
                var damage = Mathf.RoundToInt(Data.TotalDamage * multiplier);
                target.TakeDamage(damage);

                // Spawn hit effects
                var hitPoint = other.ClosestPoint(_transform.position);
                if (Data.HitVFX != null)
                    GamePoolManager.Instance?.GetParticlePrefab(Data.HitVFX, hitPoint, _transform.rotation);
                if (Data.HitSfx != null)
                    GamePoolManager.Instance?.GetWorldAudioPrefab(Data.HitSfx, hitPoint);
            }
        }

        #endregion

        #region Stuck

        /// <summary>
        /// Sticks the weapon to a surface.
        /// </summary>
        private void StickToSurface(Collision collision)
        {
            _state = ThrowableState.Stuck;
            _stuckTime = Time.time;
            
            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Embed slightly into surface
            var contact = collision.GetContact(0);
            _transform.position = contact.point + contact.normal * 0.05f;
            _transform.rotation = Quaternion.LookRotation(-contact.normal);
        }

        /// <summary>
        /// Updates stuck state - auto-return after duration.
        /// </summary>
        private void UpdateStuck()
        {
            if (Time.time - _stuckTime > stickDuration)
            {
                StartReturn();
            }
        }

        #endregion

        #region Return

        /// <summary>
        /// Starts returning the weapon to the player.
        /// </summary>
        public void StartReturn()
        {
            if (_state == ThrowableState.Returning) return;
            if (_state == ThrowableState.Held) return;

            _state = ThrowableState.Returning;
            _currentReturnSpeed = returnSpeed * 0.5f;
            
            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            
            // Set as trigger for return flight (pass through enemies, deal damage)
            if (_collider) _collider.isTrigger = true;
        }

        /// <summary>
        /// Updates return flight towards player hand.
        /// </summary>
        private void UpdateReturn()
        {
            if (_returnTarget == null && _lastHolder != null)
            {
                _returnTarget = _lastHolder;
            }

            if (_returnTarget == null)
            {
                // No target, fall to ground
                _state = ThrowableState.InFlight;
                _rb.isKinematic = false;
                return;
            }

            // Accelerate towards target
            _currentReturnSpeed = Mathf.Min(_currentReturnSpeed + returnAcceleration * Time.fixedDeltaTime, returnSpeed);
            
            var direction = (_returnTarget.position - _transform.position).normalized;
            var distance = Vector3.Distance(_transform.position, _returnTarget.position);

            // Move towards target
            _transform.position += direction * (_currentReturnSpeed * Time.fixedDeltaTime);
            
            // Rotate towards target
            if (direction.sqrMagnitude > 0.001f)
            {
                _transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Spin during return
            _transform.Rotate(Vector3.forward, spinSpeed * Time.fixedDeltaTime, Space.Self);

            // Check if caught
            if (distance <= catchDistance)
            {
                CatchWeapon();
            }
        }

        /// <summary>
        /// Catches the weapon when it returns to the player.
        /// </summary>
        private void CatchWeapon()
        {
            _state = ThrowableState.Held;
            _rb.isKinematic = true;
            
            if (_collider) _collider.isTrigger = false;

            // Haptic feedback on catch
            TriggerHapticFeedback();
        }

        /// <summary>
        /// Sets the return target for recall.
        /// </summary>
        /// <param name="target">Transform to return to.</param>
        public void SetReturnTarget(Transform target)
        {
            _returnTarget = target;
        }

        #endregion

        #region Actions

        public override void PrimaryAction()
        {
            // Throw based on current velocity
            if (_state == ThrowableState.Held && _rb != null)
            {
                Throw(_rb.linearVelocity);
            }
        }

        public override void SecondaryAction()
        {
            // Recall weapon
            if (CanRecall)
            {
                StartReturn();
            }
        }

        #endregion

        #region Lifecycle

        protected override void OnGrab(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
        {
            base.OnGrab(args);
            
            _state = ThrowableState.Held;
            _lastHolder = args.interactorObject.transform;
            _returnTarget = _lastHolder;
            
            _rb.isKinematic = true;
            if (_collider) _collider.isTrigger = false;
        }

        protected override void OnRelease(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
        {
            // Check velocity for throw
            if (_rb.linearVelocity.magnitude >= minThrowVelocity)
            {
                Throw(_rb.linearVelocity);
            }
            else
            {
                // Just dropped, not thrown
                _rb.isKinematic = false;
            }
            
            base.OnRelease(args);
        }

        protected override void OnEquipped()
        {
            base.OnEquipped();
            _state = ThrowableState.Held;
        }

        #endregion
    }
}
