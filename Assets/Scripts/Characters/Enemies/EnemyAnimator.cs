using Characters.Base;
using Constants;
using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyAnimator : AnimatorComponent
    {
        #region Animator Hashes
        
        private static readonly int LightAttackHash = Animator.StringToHash(GameConstants.LightAttackTrigger);
        private static readonly int LightAttackIndexHash = Animator.StringToHash(GameConstants.LightAttackIndex);
        private static readonly int HeavyAttackHash = Animator.StringToHash(GameConstants.HeavyAttackTrigger);
        private static readonly int HeavyAttackIndexHash = Animator.StringToHash(GameConstants.HeavyAttackIndex);
        
        private static readonly int HitLeftHash = Animator.StringToHash(GameConstants.HitLeftTrigger);
        private static readonly int HitRightHash = Animator.StringToHash(GameConstants.HitRightTrigger);
        private static readonly int HitFrontHash = Animator.StringToHash(GameConstants.HitFrontTrigger);
        
        #endregion
        
        #region Methods
        
        public void OnSpawn()
        {
            if(!Animator)
                Animator = GetComponent<Animator>();
            
            Animator.enabled = true;
            SetBool(IsMovingHash, true);
        }
        
        public void OnDespawn()
        {
            Animator.enabled = false;
        }
        
        #endregion
        
        #region Attacks

        public void PlayLightAttack()
        {
            Animator.SetInteger(LightAttackIndexHash, Random.Range(0, GameConstants.LightAttackCount));
            Animator.SetTrigger(LightAttackHash);
        }

        public void PlayHeavyAttack()
        {
            Animator.SetInteger(HeavyAttackIndexHash, Random.Range(0, GameConstants.HeavyAttackCount));
            Animator.SetTrigger(HeavyAttackHash);
        }

        #endregion

        #region Hit Reactions

        public void PlayHitReaction(Vector3 hitDirection)
        {
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