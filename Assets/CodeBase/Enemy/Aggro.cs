using System;
using System.Collections;
using UnityEngine;

namespace CodeBase.Enemy
{
    public class Aggro : MonoBehaviour
    {
        [SerializeField] private TriggerObserver _triggerObserver;
        [SerializeField] private Follow _follow;
        [SerializeField] private float _cooldown = 1f;

        private Coroutine _aggroCoroutine;
        private bool _hasAggroTarget;

        private void Start() {
            _triggerObserver.TriggerEnter += TriggerEnter;
            _triggerObserver.TriggerExit += TriggerExit;

            SwitchFollowOff();
        }

        private void TriggerEnter(Collider obj) {
            if(_hasAggroTarget) return;

            _hasAggroTarget = true; 
            
            StopAgroCoroutine();
            
            SwitchFollowOn();
        }

        private void TriggerExit(Collider obj) {
            if(!_hasAggroTarget) return;
            
            _hasAggroTarget = false;
            _aggroCoroutine = StartCoroutine(SwitchFollowOffAfterCooldown());
        }

        private IEnumerator SwitchFollowOffAfterCooldown() {
            yield return new WaitForSeconds(_cooldown);
            
            SwitchFollowOff();
        }

        private void StopAgroCoroutine() {
            if(_aggroCoroutine == null) return;
            
            StopCoroutine(_aggroCoroutine);
            _aggroCoroutine = null;
        }

        private void SwitchFollowOn() => 
            _follow.enabled = true;

        private void SwitchFollowOff() => 
            _follow.enabled = false;
    }
}