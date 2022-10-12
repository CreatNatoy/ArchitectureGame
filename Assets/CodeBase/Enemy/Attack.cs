using System;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private float _attackCooldown = 3f; 

        private IGameFactory _factory;
        private Transform _heroTransform;
        private float _currentAttackCooldown;
        private bool _isAttacking;

        private void Awake() {
            _factory = AllServices.Container.Single<IGameFactory>();
            _factory.HeroCreated += OnHeroCreated; 
        }

        private void Update() {
            UpdateCooldown();

            if (CanAttack())
                StartAttack();
        }

        private void OnAttack(){}

        private void OnAttackEnded() {
            _attackCooldown = _currentAttackCooldown;
            _isAttacking = false;
        }

        private void UpdateCooldown() {
            if (!CooldownIsUp())
                _attackCooldown -= Time.deltaTime;
        }


        private void StartAttack() {
            transform.LookAt(_heroTransform);
            _animator.PlayAttack();
            _isAttacking = false; 
        }

        private bool CanAttack() {
            return !_isAttacking && CooldownIsUp();
        }

        private bool CooldownIsUp() => _currentAttackCooldown <= 0;

        private void OnHeroCreated() => 
            _heroTransform = _factory.HeroGameObject.transform;
    }
}