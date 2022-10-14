using System;
using CodeBase.Hero;
using UnityEngine;

namespace CodeBase.UI
{
    public class ActorUI : MonoBehaviour
    {
        [SerializeField] private HpBar _hpBar;

        private HeroHealth _heroHealth;

        public void Construct(HeroHealth health) {
            _heroHealth = health;

            _heroHealth.HealthChanged += UpdateHpBar; 
        }

        private void OnDestroy() => 
            _heroHealth.HealthChanged -= UpdateHpBar;

        private void UpdateHpBar() {
           _hpBar.SetValue(_heroHealth.Current, _heroHealth.Max); 
        }
    }
}