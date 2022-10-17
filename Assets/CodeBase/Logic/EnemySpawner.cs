using System;
using CodeBase.Data;
using CodeBase.Enemy;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.StaticData;
using UnityEngine;

namespace CodeBase.Logic
{
    public class EnemySpawner : MonoBehaviour, ISavedProgress
    {
        [SerializeField] private MonsterTypeId _monsterTypeId;
        [SerializeField] private bool _slain;

        private string _id;
        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;

        private void Awake() {
            _id = GetComponent<UniqueId>().Id;
            _factory = AllServices.Container.Single<IGameFactory>();
        }

        public void LoadProgress(PlayerProgress progress) {
            if (progress.KillData.ClearedSpawners.Contains(_id))
                _slain = true;
            else
                Spawn();
        }

        private void Spawn() {
            var monster = _factory.CreateMonster(_monsterTypeId, transform);
            _enemyDeath = monster.GetComponent<EnemyDeath>();
            _enemyDeath.Happened += Slay;
        }

        private void Slay() {
            if (_enemyDeath != null)
                _enemyDeath.Happened -= Slay;

            _slain = true; 
        }

        public void UpdateProgress(PlayerProgress progress) {
            if (_slain) {
                progress.KillData.ClearedSpawners.Add(_id);
            }
        }
    }
}