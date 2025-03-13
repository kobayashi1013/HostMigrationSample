using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

namespace HostMigration
{
    public class Observer : MonoBehaviour, INetworkRunnerCallbacks
    {
        public void Awake()
        {
            if (Handler.Instance == null)
            {
                //�z�X�g�J�ڗp�̃n���h�����N��
                Handler.Instance = new Handler();

                //�ċN���p��Runner���m�ۂ��Ă���
                Handler.Instance.RunnerClone = Instantiate(this.gameObject);
                Handler.Instance.RunnerClone.SetActive(false);
                DontDestroyOnLoad(Handler.Instance.RunnerClone);

                //���O��ύX���Ă���
                gameObject.name = "Runner";
                Handler.Instance.RunnerClone.name = "RunnerClone";
            }
        }

        /// <summary>
        /// �z�X�g���ؒf�������ɌĂяo�����
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hostMigrationToken"></param>
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Handler.Instance.RebootRunner(runner, hostMigrationToken);
        }

        /// <summary>
        /// Runner���I�������Ƃ��ɌĂяo�����
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="shutdownReason"></param>
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            //���\�[�X�̉��
            if (shutdownReason != ShutdownReason.HostMigration)
            {
                Destroy(Handler.Instance.RunnerClone);
                Handler.Instance = null;
            }
        }

        /// <summary>
        /// �v���C���[�Q�����ɌĂяo�����
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            //�����v���C���[�ł���ꍇ�̓I�u�W�F�N�g�Ɍ��������蓖�Ă�
            var connectionToken = new Guid(runner.GetPlayerConnectionToken(player)).GetHashCode().ToString();
            if (Handler.Instance.ResumePlayerDict.ContainsKey(connectionToken))
            {
                //���͌��������蓖�Ă�
                var playerObj = Handler.Instance.ResumePlayerDict[connectionToken];
                playerObj.AssignInputAuthority(player);
                if (!runner.TryGetPlayerObject(player, out var _)) runner.SetPlayerObject(player, playerObj);

                //�ڑ��g�[�N����ݒ肷��
                if (!playerObj.TryGetComponent<ResumeObject>(out var resumeObject)) return;
                if (player == runner.LocalPlayer) resumeObject.token = "Host";
                else resumeObject.token = connectionToken;
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }

}