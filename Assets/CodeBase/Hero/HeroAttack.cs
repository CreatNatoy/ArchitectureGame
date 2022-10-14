using CodeBase.Data;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic;
using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroAttack : MonoBehaviour, ISavedProgressReader
    {
        [SerializeField] private HeroAnimator _heroAnimator;
        [SerializeField] private CharacterController _characterController;

        private IInputService _input;
        private static int _layerMask;
        private Collider[] _hits = new Collider[3];
        private Stats _stats;

        private void Awake() {
            _input = AllServices.Container.Single<IInputService>();

            _layerMask = 1 << LayerMask.NameToLayer("Hittable");
        }

        private void Update() {
            if(_input.IsAttackButtonUp() && !_heroAnimator.IsAttacking)
                _heroAnimator.PlayAttack();
        }

        public void OnAttack() {
            foreach (var hit in _hits) 
                hit.transform.parent.GetComponent<IHealth>().TakeDamage(_stats.Damage);
        }

        public void LoadProgress(PlayerProgress progress) => 
            _stats = progress.HeroStats;

        private int Hit() => 
            Physics.OverlapSphereNonAlloc(StartPoint() + transform.forward, _stats.DamageRadius, _hits, _layerMask);

        private Vector3 StartPoint() =>
            new (transform.position.x, _characterController.center.y / 2, transform.position.z);
    }
}