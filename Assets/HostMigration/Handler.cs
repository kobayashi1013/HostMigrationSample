using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

namespace HostMigration
{
    public class Handler
    {
        public static Handler Instance;

        public GameObject RunnerClone; //Runnerのクローンを格納しておく
        public Dictionary<string, NetworkObject> ResumePlayerDict = new Dictionary<string, NetworkObject>(); //復元プレイヤーを格納

        /// <summary>
        /// Runnerを再起動させる
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hostMigrationToken"></param>
        public async void RebootRunner(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            //接続トークンの取得
            var connectionToken = runner.GetPlayerConnectionToken(runner.LocalPlayer);

            //Runnerを停止する
            await runner.Shutdown(true, ShutdownReason.HostMigration);

            //RunnerObjectを生成する
            var runnerObject = UnityEngine.Object.Instantiate(RunnerClone); //クローンからRunnerを生成
            runnerObject.name = "Runner"; //名前を変更
            runnerObject.SetActive(true); //Runnerを有効化

            //Runnerを起動する
            runner = runnerObject.GetComponent<NetworkRunner>();
            runner.ProvideInput = true; //入力権限を付与

            var args = new StartGameArgs
            {
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume,
                ConnectionToken = connectionToken,
            };

            await runner.StartGame(args);
        }

        /// <summary>
        /// 新ホストだけに呼び出される
        /// </summary>
        /// <param name="runner"></param>
        private void HostMigrationResume(NetworkRunner runner)
        {
            ResumePlayerDict.Clear();

            foreach (NetworkObject resumeObj in runner.GetResumeSnapshotNetworkObjects())
            {
                //ResumeObjectコンポーネントがついていなければ、オブジェクトを復元しない
                if (!resumeObj.TryGetComponent<ResumeObject>(out var resumeObject)) continue;

                //旧ホストオブジェクトは復元させない
                if (resumeObject.token == "Host")
                {
                    continue;
                }
                else
                {
                    //オブジェクトの状態を復元してスポーンする
                    Vector3 position = resumeObject.position;
                    Quaternion rotation = resumeObject.rotation;

                    var spawnObj = runner.Spawn(resumeObj, position, rotation, null, (_, obj) =>
                    {
                        obj.CopyStateFrom(resumeObj);
                    });

                    //プレイヤーを記録する
                    if (resumeObject.token != "None")
                    {
                        ResumePlayerDict.Add(resumeObject.token, spawnObj);
                    }
                }
            }
        }
    }
}
