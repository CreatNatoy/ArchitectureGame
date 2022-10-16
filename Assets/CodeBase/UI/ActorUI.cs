using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.UI
{
    public class ActorUI : MonoBehaviour
    {
        [SerializeField] private HpBar _hpBar;

        private IHealth _health;

        public void Construct(IHealth health) {
            _health = health;

            _health.HealthChanged += UpdateHpBar; 
        }

        private void Start() {
            var health = GetComponent<IHealth>();
            
            if(health != null)
                Construct(health);
        }

        private void OnDestroy() => 
            _health.HealthChanged -= UpdateHpBar;

        private void UpdateHpBar() {
           _hpBar.SetValue(_health.Current, _health.Max); 
        }
    }
}