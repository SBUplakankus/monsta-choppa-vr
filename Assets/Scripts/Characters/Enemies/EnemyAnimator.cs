using Characters.Base;
using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyAnimator : AnimatorComponent
    {
        #region Animator Hashes
        
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int IsAlertHash = Animator.StringToHash("IsAlert");
        
        #endregion
        
        #region Methods
        
        public void ResetAnimator()
        {
            
        }
        
        public void InitAnimator()
        {
            SetBool(IsMovingHash, true);
        }
        
        #endregion
    }
}