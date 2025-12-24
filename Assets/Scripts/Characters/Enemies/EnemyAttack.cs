using UnityEngine;
using Weapons;

namespace Characters.Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private Transform weaponSocket;
        private WeaponData _weapon;

        public void ResetAttack()
        {
            
        }
        
        public void InitAttack(WeaponData weaponData)
        {
            _weapon = weaponData;
        }
    }
}
