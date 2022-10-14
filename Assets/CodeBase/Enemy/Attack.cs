using System.Linq;
using CodeBase.Hero;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private float _attackCooldown = 3f;
        [SerializeField] private float _cleavage = 0.5f;
        [SerializeField] private float _effectiveDistance = 0.5f;
        [SerializeField] private float _damage = 10f;

        private IGameFactory _factory;
        private Transform _heroTransform;
        private float _currentAttackCooldown;
        private bool _isAttacking;
        private Collider[] _hits = new Collider[1];
        private int _layerMask;
        private bool _attackIsActive;

        private void Awake() {
            _factory = AllServices.Container.Single<IGameFactory>();
            _layerMask = 1 << LayerMask.NameToLayer("Player");
            _factory.HeroCreated += OnHeroCreated;
        }

        private void Update() {
            UpdateCooldown();

            if (CanAttack())
                StartAttack();
        }

        private void OnAttack() {
            if (Hit(out var hit)) {
                PhysicsDebug.DrawDebug(StartPoint(), _cleavage, 1);
                hit.transform.GetComponent<IHealth>().TakeDamage(_damage);
            }
        }

        private void OnAttackEnded() {
            _currentAttackCooldown = _attackCooldown;
            _isAttacking = false;
        }

        public void DisableAttack() => _attackIsActive = false;

        public void EnableAttack() => _attackIsActive = true;
        
        private bool Hit(out Collider hit) {
            var hitCount = Physics.OverlapSphereNonAlloc(StartPoint(), _cleavage, _hits, _layerMask);

            hit = _hits.FirstOrDefault();

            return hitCount > 0;
        }

        private Vector3 StartPoint() {
            return new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) +
                   transform.forward * _effectiveDistance;
        }

        private void UpdateCooldown() {
            if (!CooldownIsUp())
                _currentAttackCooldown -= Time.deltaTime;
        }


        private void StartAttack() {
            transform.LookAt(_heroTransform);
            _animator.PlayAttack();
            _isAttacking = true;
        }

        private bool CanAttack() => _attackIsActive && !_isAttacking && CooldownIsUp();

        private bool CooldownIsUp() => _currentAttackCooldown <= 0f;

        private void OnHeroCreated() => _heroTransform = _factory.HeroGameObject.transform;
    }
}