using UnityEngine;

namespace Characters.Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        private int _damage;
        private float _attackRate;

        public void ResetAttack()
        {
            
        }
        
        public void InitAttack(int damage, int attackRate)
        {
            _damage = damage;
            _attackRate = attackRate;
        }
    }
}
