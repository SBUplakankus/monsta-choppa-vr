using Characters.Base;
using Characters.Enemies;
using Constants;
using Interfaces;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Detects weapon collisions and processes hits on damageable targets.
    /// Supports velocity-based damage multipliers for melee weapons and
    /// directional hit detection for enemy hit reactions.
    /// </summary>
    public class WeaponHitbox : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private XRWeaponBase weapon;

        [Header("Hit Detection")]
        [SerializeField] private float hitCooldown = GameConstants.InvincibilityDuration;
        
        private float _lastHitTime;
        private Collider _lastHitCollider;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            weapon = GetComponentInParent<XRWeaponBase>();
        }

        private void OnTriggerEnter(Collider other)
        {
            ProcessHit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            // Allow continuous hits on different targets or after cooldown
            if (other != _lastHitCollider || Time.time >= _lastHitTime + hitCooldown)
            {
                ProcessHit(other);
            }
        }

        #endregion

        #region Hit Processing

        /// <summary>
        /// Processes a hit on a collider, applying damage if valid.
        /// </summary>
        /// <param name="other">The collider that was hit.</param>
        private void ProcessHit(Collider other)
        {
            if (!weapon || !weapon.IsActive || !weapon.CanAttack)
                return;

            // Check for melee weapon velocity requirements
            if (weapon is MeleeXRWeapon meleeWeapon && !meleeWeapon.IsSwinging)
                return;

            if (!other.TryGetComponent<IDamageable>(out var target))
                return;

            // Calculate hit direction for directional reactions
            var hitDirection = CalculateHitDirection(other.transform);
            var hitPoint = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(hitDirection);

            // Get damage multiplier from melee weapon velocity
            var damageMultiplier = 1f;
            if (weapon is MeleeXRWeapon melee)
            {
                damageMultiplier = melee.VelocityDamageMultiplier;
            }

            // Process the hit with velocity-based damage
            weapon.ProcessHit(target, hitPoint, hitRotation, damageMultiplier);

            // Trigger hit reaction on enemy
            TriggerHitReaction(other, hitDirection);

            // Record hit for cooldown
            _lastHitTime = Time.time;
            _lastHitCollider = other;
        }

        /// <summary>
        /// Calculates the direction of the hit from the weapon to the target.
        /// </summary>
        /// <param name="targetTransform">The transform of the hit target.</param>
        /// <returns>Normalized direction vector from weapon to target.</returns>
        private Vector3 CalculateHitDirection(Transform targetTransform)
        {
            var direction = (targetTransform.position - transform.position).normalized;
            
            // If we have a rigidbody, use velocity direction for more accurate hit direction
            if (weapon.TryGetComponent<Rigidbody>(out var rb) && rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                direction = rb.linearVelocity.normalized;
            }
            
            return direction;
        }

        /// <summary>
        /// Triggers a hit reaction animation on the enemy if applicable.
        /// </summary>
        /// <param name="other">The collider that was hit.</param>
        /// <param name="hitDirection">The direction of the hit.</param>
        private void TriggerHitReaction(Collider other, Vector3 hitDirection)
        {
            // Try to get enemy animator for hit reaction
            var enemyAnimator = other.GetComponentInParent<EnemyAnimator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.PlayHitReaction(hitDirection);
            }
        }

        #endregion
    }
}
