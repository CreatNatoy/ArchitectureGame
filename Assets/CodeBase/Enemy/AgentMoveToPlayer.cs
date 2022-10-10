using System;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Enemy
{
    public class AgentMoveToPlayer : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _minimalDistance = 1f;
        
        private IGameFactory _gameFactory;
        private Transform _heroTransform;

        private void Start() {
            _gameFactory = AllServices.Container.Single<IGameFactory>();

            if (_gameFactory.HeroGameObject != null)
                InitializationHeroTransform();
            else
                _gameFactory.HeroCreated += HeroCreated;
        }

        private void Update() {
            if (Initialized() && HeroNotReached())
                _agent.destination = _heroTransform.position; 
        }

        private bool Initialized() => _heroTransform != null;

        private void HeroCreated() => InitializationHeroTransform();

        private void InitializationHeroTransform() => 
            _heroTransform = _gameFactory.HeroGameObject.transform;

        private bool HeroNotReached() => 
            Vector3.Distance(_agent.transform.position, _heroTransform.position) >= _minimalDistance;
    }
}