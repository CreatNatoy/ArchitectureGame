using System;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroDeath : MonoBehaviour
    {
       [SerializeField] private HeroHealth _health;
       [SerializeField] private HeroMove _move;
       [SerializeField] private HeroAnimator _animator;
       [SerializeField] private GameObject _deathFx; 
       
        private bool _isDead;

        private void Start() => _health.HealthChanged += HealthChanged;

        private void OnDestroy() => _health.HealthChanged -= HealthChanged;

        private void HealthChanged() {
            if (!_isDead && _health.Current <= 0)
                Die();
        }

        private void Die() {
            _isDead = true;
            
            _move.enabled = false;
            _animator.PlayDeath();

            Instantiate(_deathFx, transform.position, Quaternion.identity); 
        }
    }
}