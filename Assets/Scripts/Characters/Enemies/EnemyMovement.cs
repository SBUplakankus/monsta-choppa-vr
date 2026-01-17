using Constants;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemies
{
    /// <summary>
    /// AI states for enemy movement behavior.
    /// </summary>
    public enum EnemyAIState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }
    
    /// <summary>
    /// Handles enemy navigation and AI movement using NavMeshAgent.
    /// Manages player tracking, pursuit behavior, and attack range detection.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour
    {
        #region Fields

        [Header("AI Settings")]
        [SerializeField] private float attackRange = GameConstants.DefaultMeleeAttackRange;
        [SerializeField] private float stoppingDistance = 1.5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float pathUpdateInterval = 0.2f;

        private EnemyAnimator _animator;
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigidbody;
        
        private Transform _target;
        private EnemyAIState _currentState = EnemyAIState.Idle;
        private float _lastPathUpdateTime;
        private float _maxSpeed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current AI state of the enemy.
        /// </summary>
        public EnemyAIState CurrentState => _currentState;

        /// <summary>
        /// Returns true if the enemy is within attack range of the target.
        /// </summary>
        public bool IsInAttackRange => _target && Vector3.Distance(transform.position, _target.position) <= attackRange;

        /// <summary>
        /// Returns true if the enemy has a valid target.
        /// </summary>
        public bool HasTarget => _target != null;

        /// <summary>
        /// Gets the attack range of this enemy.
        /// </summary>
        public float AttackRange => attackRange;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the enemy movement component when spawned from the pool.
        /// </summary>
        /// <param name="speed">Movement speed for this enemy.</param>
        /// <param name="animator">Reference to the enemy's animator component.</param>
        public void OnSpawn(float speed, EnemyAnimator animator)
        {
            if (!_navMeshAgent)
                _navMeshAgent = GetComponent<NavMeshAgent>();

            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = speed;
            _navMeshAgent.stoppingDistance = stoppingDistance;
            _maxSpeed = speed;

            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody>();

            if (!_animator)
                _animator = animator;

            _rigidbody.isKinematic = false;
            _currentState = EnemyAIState.Idle;
            
            // Find player target
            FindTarget();
        }

        /// <summary>
        /// Cleans up the movement component when returned to the pool.
        /// </summary>
        public void OnDespawn()
        {
            _currentState = EnemyAIState.Dead;
            _target = null;
            
            if (_navMeshAgent && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.enabled = false;
            }

            if (_rigidbody)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }
        }

        #endregion

        #region AI Update

        /// <summary>
        /// Updates the AI state machine and movement. Called from EnemyController.
        /// </summary>
        public void UpdateAI()
        {
            if (_currentState == EnemyAIState.Dead)
                return;

            if (!HasTarget)
            {
                FindTarget();
                if (!HasTarget)
                {
                    SetState(EnemyAIState.Idle);
                    return;
                }
            }

            // Update path periodically for performance
            if (Time.time - _lastPathUpdateTime >= pathUpdateInterval)
            {
                UpdatePath();
                _lastPathUpdateTime = Time.time;
            }

            // State-based behavior
            if (IsInAttackRange)
            {
                SetState(EnemyAIState.Attacking);
                StopMovement();
                RotateTowardsTarget();
            }
            else if (_currentState != EnemyAIState.Attacking || !_animator.IsAttacking)
            {
                SetState(EnemyAIState.Chasing);
                ChaseTarget();
            }

            UpdateAnimatorSpeed();
        }

        /// <summary>
        /// Sets the AI to attacking state and stops movement.
        /// </summary>
        public void SetAttacking()
        {
            SetState(EnemyAIState.Attacking);
            StopMovement();
        }

        /// <summary>
        /// Sets the AI to dead state.
        /// </summary>
        public void SetDead()
        {
            SetState(EnemyAIState.Dead);
            StopMovement();
        }

        #endregion

        #region Movement

        /// <summary>
        /// Finds the player target in the scene.
        /// </summary>
        private void FindTarget()
        {
            // Find player by tag - standard Unity pattern for VR player rigs
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                _target = player.transform;
            }
        }

        /// <summary>
        /// Updates the NavMesh path to the target.
        /// </summary>
        private void UpdatePath()
        {
            if (!_navMeshAgent || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh)
                return;

            if (_target && _currentState == EnemyAIState.Chasing)
            {
                _navMeshAgent.SetDestination(_target.position);
            }
        }

        /// <summary>
        /// Chases the target using NavMeshAgent.
        /// </summary>
        private void ChaseTarget()
        {
            if (!_navMeshAgent || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh)
                return;

            _navMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Stops all movement.
        /// </summary>
        private void StopMovement()
        {
            if (!_navMeshAgent || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh)
                return;

            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }

        /// <summary>
        /// Rotates the enemy to face the target smoothly.
        /// </summary>
        private void RotateTowardsTarget()
        {
            if (!_target)
                return;

            var direction = (_target.position - transform.position).normalized;
            direction.y = 0; // Keep rotation on horizontal plane
            
            if (direction.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Updates the animator with current movement speed for blend tree.
        /// </summary>
        private void UpdateAnimatorSpeed()
        {
            if (!_animator || !_navMeshAgent)
                return;

            var normalizedSpeed = _navMeshAgent.velocity.magnitude / _maxSpeed;
            _animator.UpdateMovementSpeed(Mathf.Clamp01(normalizedSpeed));
        }

        /// <summary>
        /// Sets the current AI state.
        /// </summary>
        /// <param name="newState">The new AI state.</param>
        private void SetState(EnemyAIState newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        #endregion
    }
}
