using Characters.Base;
using Constants;
using UnityEngine;
using Weapons;

namespace Characters.Enemies
{
    /// <summary>
    /// Handles enemy attack behavior including cooldowns, damage dealing, and player detection.
    /// Works with EnemyAnimator for attack animations.
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        #region Fields

        [Header("Attack Settings")]
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private float attackCooldown = GameConstants.DefaultAttackCooldown;
        [SerializeField] private LayerMask playerLayer;
        
        [Header("Hitbox")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 0.5f;

        private WeaponData _weapon;
        private EnemyAnimator _animator;
        private EnemyMovement _movement;
        
        private float _lastAttackTime;
        private bool _isAttacking;
        private bool _canDealDamage;

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the enemy can perform an attack (cooldown has elapsed).
        /// </summary>
        public bool CanAttack => Time.time >= _lastAttackTime + attackCooldown && !_isAttacking;

        /// <summary>
        /// Returns true if the enemy is currently attacking.
        /// </summary>
        public bool IsAttacking => _isAttacking;

        /// <summary>
        /// Gets the attack damage from the equipped weapon or a default value.
        /// </summary>
        public int AttackDamage => _weapon != null ? _weapon.TotalDamage : 10;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the attack component with weapon data and references.
        /// </summary>
        /// <param name="weaponData">The weapon data for this enemy.</param>
        /// <param name="animator">Reference to the enemy's animator.</param>
        /// <param name="movement">Reference to the enemy's movement component.</param>
        public void InitAttack(WeaponData weaponData, EnemyAnimator animator, EnemyMovement movement)
        {
            _weapon = weaponData;
            _animator = animator;
            _movement = movement;
            
            if (_weapon != null)
            {
                attackCooldown = _weapon.AttackCooldown;
            }
        }

        /// <summary>
        /// Resets the attack state when enemy is despawned.
        /// </summary>
        public void ResetAttack()
        {
            _isAttacking = false;
            _canDealDamage = false;
            _lastAttackTime = 0f;
        }

        #endregion

        #region Attack Logic

        /// <summary>
        /// Attempts to perform an attack. Returns true if attack was initiated.
        /// </summary>
        /// <returns>True if attack started, false if on cooldown or already attacking.</returns>
        public bool TryAttack()
        {
            if (!CanAttack)
                return false;

            StartAttack();
            return true;
        }

        /// <summary>
        /// Initiates an attack sequence.
        /// </summary>
        private void StartAttack()
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;
            _canDealDamage = true;
            
            // Stop movement during attack
            _movement?.SetAttacking();
            
            // Play attack animation (randomly choose light or heavy)
            if (_animator)
            {
                if (Random.value > 0.3f)
                    _animator.PlayLightAttack();
                else
                    _animator.PlayHeavyAttack();
            }
        }

        /// <summary>
        /// Called by animation event during the attack animation's damage frame.
        /// Checks for player in attack range and applies damage.
        /// </summary>
        public void OnAttackHit()
        {
            if (!_canDealDamage)
                return;

            _canDealDamage = false;
            
            var hitPoint = attackPoint ? attackPoint.position : transform.position + transform.forward;
            var colliders = Physics.OverlapSphere(hitPoint, attackRadius, playerLayer);
            
            foreach (var col in colliders)
            {
                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(AttackDamage);
                }
            }
        }

        /// <summary>
        /// Called by animation event when attack animation ends.
        /// </summary>
        public void OnAttackEnd()
        {
            _isAttacking = false;
            _canDealDamage = false;
            _animator?.OnAttackEnd();
        }

        #endregion

        #region Debug

        private void OnDrawGizmosSelected()
        {
            var hitPoint = attackPoint ? attackPoint.position : transform.position + transform.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint, attackRadius);
        }

        #endregion
    }
}
