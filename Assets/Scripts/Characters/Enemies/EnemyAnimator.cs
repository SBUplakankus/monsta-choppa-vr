using Characters.Base;
using Constants;
using UnityEngine;

namespace Characters.Enemies
{
    /// <summary>
    /// Handles enemy animations using a layered approach:
    /// - Base Layer: Synty locomotion pack for lower body movement
    /// - Upper Body Layer: Mixamo animations for combat (attacks, hit reactions)
    /// </summary>
    public class EnemyAnimator : AnimatorComponent
    {
        #region Animator Hashes
        
        // Attack Hashes (Upper Body Layer - Mixamo)
        private static readonly int LightAttackHash = Animator.StringToHash(GameConstants.LightAttackTrigger);
        private static readonly int LightAttackIndexHash = Animator.StringToHash(GameConstants.LightAttackIndex);
        private static readonly int HeavyAttackHash = Animator.StringToHash(GameConstants.HeavyAttackTrigger);
        private static readonly int HeavyAttackIndexHash = Animator.StringToHash(GameConstants.HeavyAttackIndex);
        
        // Hit Reaction Hashes (Upper Body Layer - Mixamo)
        private static readonly int HitLeftHash = Animator.StringToHash(GameConstants.HitLeftTrigger);
        private static readonly int HitRightHash = Animator.StringToHash(GameConstants.HitRightTrigger);
        private static readonly int HitFrontHash = Animator.StringToHash(GameConstants.HitFrontTrigger);
        
        // State Hashes
        private static readonly int IsAttackingHash = Animator.StringToHash(GameConstants.IsAttackingParam);
        private static readonly int WeaponTypeHash = Animator.StringToHash(GameConstants.WeaponTypeParam);
        
        #endregion

        #region Layer Settings

        [Header("Animation Layers")]
        [SerializeField] private int upperBodyLayerIndex = 1;
        [SerializeField] private float upperBodyLayerWeight = 1f;

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the enemy is currently playing an attack animation.
        /// </summary>
        public bool IsAttacking => Animator && Animator.GetBool(IsAttackingHash);

        #endregion
        
        #region Methods
        
        public void OnSpawn()
        {
            if(!Animator)
                Animator = GetComponent<Animator>();
            
            Animator.enabled = true;
            Animator.Rebind();
            Animator.Update(0f);
            
            // Set up layer weights - upper body layer for Mixamo combat animations
            if (Animator.layerCount > upperBodyLayerIndex)
            {
                Animator.SetLayerWeight(upperBodyLayerIndex, upperBodyLayerWeight);
            }
            
            SetBool(IsMovingHash, true);
            SetBool(IsAttackingHash, false);
        }
        
        public void OnDespawn()
        {
            if (Animator)
            {
                SetBool(IsMovingHash, false);
                SetBool(IsAttackingHash, false);
                Animator.enabled = false;
            }
        }

        /// <summary>
        /// Sets the weapon type for animation layer switching.
        /// Used by Synty locomotion pack for weapon-specific movement animations.
        /// </summary>
        /// <param name="weaponType">The weapon type key from GameConstants.</param>
        public void SetWeaponType(int weaponType)
        {
            if (Animator)
                Animator.SetInteger(WeaponTypeHash, weaponType);
        }

        /// <summary>
        /// Updates the movement speed for Synty locomotion blend tree (lower body).
        /// </summary>
        /// <param name="normalizedSpeed">Speed value between 0 and 1 for blend tree.</param>
        public void UpdateMovementSpeed(float normalizedSpeed)
        {
            if (Animator)
            {
                SetSpeed(normalizedSpeed);
                SetBool(IsMovingHash, normalizedSpeed > 0.01f);
            }
        }

        /// <summary>
        /// Sets the upper body layer weight for blending combat animations.
        /// </summary>
        /// <param name="weight">Weight between 0 and 1.</param>
        public void SetUpperBodyLayerWeight(float weight)
        {
            if (Animator && Animator.layerCount > upperBodyLayerIndex)
            {
                Animator.SetLayerWeight(upperBodyLayerIndex, Mathf.Clamp01(weight));
            }
        }
        
        #endregion
        
        #region Attacks (Upper Body Layer - Mixamo)

        /// <summary>
        /// Plays a random light attack animation on the upper body layer.
        /// Lower body continues with Synty locomotion.
        /// </summary>
        public void PlayLightAttack()
        {
            if (!Animator) return;
            
            Animator.SetInteger(LightAttackIndexHash, Random.Range(0, GameConstants.LightAttackCount));
            Animator.SetBool(IsAttackingHash, true);
            Animator.SetTrigger(LightAttackHash);
        }

        /// <summary>
        /// Plays a random heavy attack animation on the upper body layer.
        /// Lower body continues with Synty locomotion.
        /// </summary>
        public void PlayHeavyAttack()
        {
            if (!Animator) return;
            
            Animator.SetInteger(HeavyAttackIndexHash, Random.Range(0, GameConstants.HeavyAttackCount));
            Animator.SetBool(IsAttackingHash, true);
            Animator.SetTrigger(HeavyAttackHash);
        }

        /// <summary>
        /// Called by animation event when attack animation ends.
        /// </summary>
        public void OnAttackEnd()
        {
            if (Animator)
                Animator.SetBool(IsAttackingHash, false);
        }

        #endregion

        #region Hit Reactions (Upper Body Layer - Mixamo)

        /// <summary>
        /// Plays a hit reaction animation on the upper body layer based on hit direction.
        /// Lower body continues with Synty locomotion for smooth movement.
        /// </summary>
        /// <param name="hitDirection">World-space direction the hit came from.</param>
        public void PlayHitReaction(Vector3 hitDirection)
        {
            if (!Animator) return;
            
            // Convert hit direction to local space
            var localDir = transform.InverseTransformDirection(hitDirection);

            switch (localDir.x)
            {
                case < -0.3f:
                    Animator.SetTrigger(HitLeftHash);
                    break;
                case > 0.3f:
                    Animator.SetTrigger(HitRightHash);
                    break;
                default:
                    Animator.SetTrigger(HitFrontHash);
                    break;
            }
        }

        #endregion
    }
}