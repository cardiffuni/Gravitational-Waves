using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Mirror;
using System.Linq;
using Game.Teams;
using Game.Extensions;
using Game.Players;

namespace Game.Network {
    public class NetworkLayer : NetworkBehaviour {
        internal static GameObject GameObject;

        [Tooltip("Trigger Zone Prefab")]
        public GameObject Zone;

        [Scene]
        [Tooltip("Add all server sub-scenes to this list")]
        public List<string> subScenesServer;

        [Scene]
        [Tooltip("Add client sub-scene")]
        public string subSceneClient;

        [Scene]
        public string playerView;

        [Scene]
        public string serverView;

        /// <summary>
        /// The default prefab to be used to create player objects on the server.
        /// <para>Player objects are created in the default handler for AddPlayer() on the server. Implementing OnServerAddPlayer overrides this behaviour.</para>
        /// </summary>
        [Header("Player Object")]
        [Tooltip("Prefab of the player object. Prefab must have a Network Identity component. May be an empty game object or a full avatar.")]
        public GameObject playerPrefab;

        private static NetworkingController NetCont { get => Managers.NetworkingManager.NetworkingController; }

        public SyncClassList<Team> Teams { get => TeamManager.Teams; set => TeamManager.Teams = value; }
        public SyncClassList<Player> Players { get => PlayerManager.Players; set => PlayerManager.Players = value; }

        public bool Ready { get; private set; }

        private void OnEnable() {
            if (!Ready) {
                Debug.LogFormat("NetworkLayer OnEnable");
                //Debug.LogFormat("netId NetworkLayer: {0}", netIdentity.netId);
                //Debug.LogFormat("Spawned: {0}", NetworkIdentity.spawned.Keys.ToList());
                //Debug.Log(NetworkIdentity.spawned.Keys.ToList());
                NetCont.SetPlayerPrefab(playerPrefab);
                //NetCont.SetPlayerPrefab(AssetManager.Prefab("Player Character"));
                NetCont.Zone = Zone;
                NetCont.autoCreatePlayer = true;
                NetCont.PlayerView = playerView;
                NetCont.ServerView = serverView;
                Ready = true;
            }
        }

        // Start is called before the first frame update
        void Start() {
            if (isServer) {
                StartCoroutine(StartServer());
            }
        }

        //private float nextActionTime = 0.0f;
        //private float period = 10.0f;

        // Update is called once per frame
        void Update() {

            //if (Time.time > nextActionTime) {
            //    nextActionTime += period;
            //    Debug.LogFormat("Team size: {0}", TeamManager.Teams.Count);
            //    foreach (Team team in TeamManager.Teams) {
            //        Debug.LogFormat("Team: Name {0}, Id {1}, Score {2}, GenCode {3}", team.Name, team.ID, team.Score, team.GenCode);
            //    }
            //}
        }

        IEnumerator StartServer() {
            Transport.activeTransport.enabled = false;
            yield return MySceneManager.LoadSubScenes(subScenesServer.Prepend(serverView).ToList());
            NetCont.ClientsLoadScene(subSceneClient, SceneOperation.LoadAdditive);
            Transport.activeTransport.enabled = true;
        }
    }
}