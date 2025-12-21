using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemies
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;

        public void InitMovement(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        public void ResetMovement()
        {
            
        }
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}
