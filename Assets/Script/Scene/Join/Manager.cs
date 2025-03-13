using System;
using UnityEngine;
using Fusion;

namespace Scene.Join
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private NetworkRunner _runnerPrefab;
        public async void OnJoin()
        {
            var runner = Instantiate(_runnerPrefab);
            runner.ProvideInput = true;

            var args = new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                Scene = SceneRef.FromIndex(1),
                SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
                SessionName = "test",
                PlayerCount = 2,
                ConnectionToken = Guid.NewGuid().ToByteArray(), //HostMigration‚É•K—v
            };

            await runner.StartGame(args);
        }
    }
}
