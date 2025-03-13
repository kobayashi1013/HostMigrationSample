using UnityEngine;
using Fusion;
using Network;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class View : NetworkBehaviour
    {
        [SerializeField] private float _speed = 0;
        private CharacterController _characterController;

        public override void Spawned()
        {
            if (Object.HasStateAuthority == false) return;
            _characterController = GetComponent<CharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority == false) return;
            if (GetInput(out NetworkInputData data))
            {
                data.axis0.Normalize();
                Vector3 direction = new Vector3(data.axis0.x, 0, data.axis0.y);
                Vector3 movement = direction * _speed;
                _characterController.Move(direction * Runner.DeltaTime);
            }
        }
    }
}
