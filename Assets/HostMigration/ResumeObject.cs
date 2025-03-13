using System;
using UnityEngine;
using Fusion;

namespace HostMigration
{
    public class ResumeObject : NetworkBehaviour
    {
        [Networked] [HideInInspector] public string token { get; set; } = "None";//�I�u�W�F�N�g�̏��L�҂�ݒ肷��
        [Networked] [HideInInspector] public Vector3 position { get; set; } //Transform���L�^����
        [Networked] [HideInInspector] public Quaternion rotation { get; set; }

        public override void Spawned()
        {
            if (!Runner.IsServer) return;

            //HostMigration�Ńv���C���[���������ꂽ�ꍇ�APlayerJoined�Ȃǂŕ������ꂽ�v���C���[�͍폜����
            if (!Runner.IsResume) //HostMigration���ł͂Ȃ�
            {
                //�v���C���[�I�u�W�F�N�g�����łɂ���ꍇ�i�����v���C���[�j�͍폜
                var connectionToken = new Guid(Runner.GetPlayerConnectionToken(Object.InputAuthority)).GetHashCode().ToString();
                if (Handler.Instance.ResumePlayerDict.ContainsKey(connectionToken))
                {
                    Runner.Despawn(Object);
                }
                else //���ꂪ���߂Ẵv���C���[�I�u�W�F�N�g�ł���ꍇ�i�V�K�v���C���[�j��Runner�ɓo�^
                {
                    if (!Runner.TryGetPlayerObject(Object.InputAuthority, out var _))
                        Runner.SetPlayerObject(Object.InputAuthority, Object);

                    //�ڑ��g�[�N����ݒ肷��
                    if (Object.InputAuthority == Runner.LocalPlayer) token = "Host";
                    else token = connectionToken;
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            position = transform.position;
            rotation = transform.rotation;
        }
    }
}
