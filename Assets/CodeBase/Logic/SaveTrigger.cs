using System;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace CodeBase.Logic
{
    public class SaveTrigger : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collider;
        
        private ISaveLoadService _saveLoadService;

        private void Awake() {
            _saveLoadService = AllServices.Container.Single<ISaveLoadService>();
        }

        private void OnTriggerEnter(Collider other) {
            _saveLoadService.SaveProgress();
            Debug.Log("Progress Saved");
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos() {
            if(!_collider) return;

            Gizmos.color = new Color32(35, 200, 35, 120);
            Gizmos.DrawCube(transform.position + _collider.center, _collider.size);
        }
    }
}