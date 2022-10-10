using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Enemy
{
    public class AnimateAlongAgent : MonoBehaviour
    {

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private float _minimalVelocity = 0.1f;
        
        private void Update() {
            if(ShouldMove())
                _animator.Move(_agent.velocity.magnitude);
            else
                _animator.StopMoving();
        }

        private bool ShouldMove() => 
            _agent.velocity.magnitude > _minimalVelocity && _agent.remainingDistance > _agent.radius;
    }
}