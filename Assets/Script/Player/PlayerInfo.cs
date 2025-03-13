using UnityEngine;
using Fusion;

namespace Player
{
    public class PlayerInfo : NetworkBehaviour
    {
        [Networked] public int debugNumber { get; set; } = 0;

        public override void Spawned()
        {
            if (!Runner.IsServer || Runner.IsResume) return;
            debugNumber = Random.Range(0, 100);
        }
    }
}
