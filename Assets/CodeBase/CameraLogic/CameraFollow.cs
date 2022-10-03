using System;
using UnityEngine;

namespace CodeBase.CameraLogic
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float _rotationAngleX = 55;
        [SerializeField] private int _distance = 10;
        [SerializeField] private float _offsetY = 0.5f;
        [SerializeField] private Transform _following;

        private void LateUpdate() {
            if (_following == null)
                return;
            var rotation = Quaternion.Euler(_rotationAngleX, 0, 0);
            var position = rotation * new Vector3(0, 0, -_distance) + FollowingPointPosition();
            transform.rotation = rotation;
            transform.position = position;
        }

        public void Follow(GameObject following) => _following = following.transform;

        private Vector3 FollowingPointPosition() {
            var followingPosition = _following.position;
            followingPosition.y += _offsetY;
            return followingPosition;
        }
    }
}