using System.Collections.Generic;
using UnityEngine;

namespace Characters.Base
{
    public abstract class AnimatorComponent : MonoBehaviour
    {
        protected Animator Animator;
        
        protected static readonly int SpeedHash = Animator.StringToHash("Speed");
        protected static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        
        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
        }
        
        public virtual void SetSpeed(float speed)
        {
            Animator.SetFloat(SpeedHash, speed);
        }
        
        public virtual void SetBool(int param, bool value)
        {
            Animator.SetBool(param, value);
        }
        
        public virtual void SetTrigger(int param)
        {
            Animator.SetTrigger(param);
        }
    }
}
