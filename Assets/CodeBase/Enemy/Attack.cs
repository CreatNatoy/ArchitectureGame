using System.Linq;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class Attack : MonoBehaviour
    {
        [SerializeField] private EnemyAnimator _animator;
        [SerializeField] private float _attackCooldown = 3f;
        [SerializeField] private float _cleavage = 0.5f;
        [SerializeField] private float _effectiveDistance = 0.5f;

        private IGameFactory _factory;
        private Transform _heroTransform;
        private float _currentAttackCooldown;
        private bool _isAttacking;
        private Collider[] _hits = new Collider[1];
        private int _layerMask;

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
            }
        }

        private bool Hit(out Collider hit) {
            var hitCount = Physics.OverlapSphereNonAlloc(StartPoint(), _cleavage, _hits, _layerMask);

            hit = _hits.FirstOrDefault();

            return hitCount > 0;
        }

        private Vector3 StartPoint() {
            return new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) +
                   transform.forward * _effectiveDistance;
        }

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

        private void OnHeroCreated() => _heroTransform = _factory.HeroGameObject.transform;
    }
}