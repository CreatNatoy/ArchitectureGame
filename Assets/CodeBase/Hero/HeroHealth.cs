﻿using CodeBase.Data;
using CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace CodeBase.Hero
{
    public class HeroHealth : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private HeroAnimator _animator;
        
        private State _state;

        public float Current {
            get => _state.CurrentHP;
            set => _state.CurrentHP = value;

        }

        public float Max {
            get => _state.MaxHP;
            set => _state.MaxHP = value;
        }

        public void LoadProgress(PlayerProgress progress) {
            _state = progress.HeroState;
        }

        public void UpdateProgress(PlayerProgress progress) {
            progress.HeroState.CurrentHP = Current;
            progress.HeroState.MaxHP = Max;
        }

        public void TakeDamage(float damage) {
            if(Current <= 0) return;

            Current -= damage;
            _animator.PlayHit();
        }
    }
}