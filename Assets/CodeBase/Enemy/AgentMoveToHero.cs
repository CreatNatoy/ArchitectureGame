using CodeBase.Infrastructure.Factory;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentMoveToHero : Follow
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _minimalDistance = 1f;
        
        private IGameFactory _gameFactory;
        private Transform _heroTransform;

        public void Construct(Transform heroTransform) => 
            _heroTransform = heroTransform;

        private void Update() {
            SetDestinationForAgent();
        }

        private void SetDestinationForAgent() {
            if (IsHeroNotReached() && _heroTransform)
                _agent.destination = _heroTransform.position;
        }

        private bool IsHeroNotReached() => 
            Vector3.Distance(_agent.transform.position, _heroTransform.position) >= _minimalDistance;
    }
}