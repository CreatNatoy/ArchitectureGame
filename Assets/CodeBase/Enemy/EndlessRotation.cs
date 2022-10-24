using UnityEngine;

namespace CodeBase.Enemy
{
  public class EndlessRotation : MonoBehaviour
  {
    [SerializeField] private float _speed = 1.0f;

    private void Update() => 
      transform.rotation *= Quaternion.Euler(0, _speed*Time.deltaTime, 0);
  }
}