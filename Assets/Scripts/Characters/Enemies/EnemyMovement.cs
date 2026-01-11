using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemies
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigidbody;

        // ReSharper disable Unity.PerformanceAnalysis
        public void OnSpawn(float speed)
        {
            if(!_navMeshAgent)
                _navMeshAgent = GetComponent<NavMeshAgent>();
            
            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = speed;

            if(!_rigidbody)
                _rigidbody = GetComponent<Rigidbody>();
            
            _rigidbody.isKinematic = false;
        }

        public void OnDespawn()
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
            
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}
