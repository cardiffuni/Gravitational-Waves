using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using Game.Managers.Controllers;
using Game.Network;
using Game.Players;
using Game.Tasks;
using Game.Teams;
using Game.Utility;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers {
    public class NetworkingController : Mirror.NetworkManager {

        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkingController));

        public bool isClient => mode == Mirror.NetworkManagerMode.ClientOnly;
        public bool isServer => isServerOnly || isHost;
        public bool isServerOnly => mode == Mirror.NetworkManagerMode.ServerOnly;
        public bool isHost => mode == Mirror.NetworkManagerMode.Host;

        public KcpTransport KcpTransport { get; private set; }
        public NetworkControllerHUD NetworkControllerHUD { get; private set; }

        public string PlayerView { get;  set; }
        public string ServerView { get; set; }
        public string GameLobby { get; set; }

        public GameObject Zone { get; set; }

        public static List<UnityEngine.AsyncOperation> loadingSubScenesAsync;

        #region Unity Callbacks

        public override void Awake() {
            dontDestroyOnLoad = false;
            autoCreatePlayer = false;
            showDebugMessages = true;
            networkAddress = "localhost";
            KcpTransport = gameObject.GetOrAddComponent<KcpTransport>();
            KcpTransport.debugGUI = false;
            //NetworkControllerHUD = gameObject.GetOrAddComponent<NetworkControllerHUD>();
            transport = KcpTransport;
            GameLobby = "GameLobby";


            base.Awake();
        }

        public virtual void Start() {
            NetworkingManager.PreLoad();
            // headless mode? then start the server
            // can't do this in Awake because Awake is for initialization.
            // some transports might not be ready until Start.
            //
            // (tick rate is applied in StartServer!)
#if UNITY_SERVER
            if (autoStartServerBuild)
            {
                StartServer();
            }
#endif
        }

        // NetworkIdentity.UNetStaticUpdate is called from UnityEngine while LLAPI network is active.
        // If we want TCP then we need to call it manually. Probably best from NetworkManager, although this means that we can't use NetworkServer/NetworkClient without a NetworkManager invoking Update anymore.
        /// <summary>
        /// virtual so that inheriting classes' LateUpdate() can call base.LateUpdate() too
        /// </summary>
        public override void LateUpdate() {
            UpdateSubscenes();
            base.LateUpdate();
        }

        #endregion

        protected override void RegisterServerMessages() {
            Debug.LogFormat("RegisterServerMessages");
            NetworkServer.RegisterHandler<TaskUpdate>(OnTaskUpdateInternal, false);
            NetworkServer.RegisterHandler<PlayerScore>(OnServerPlayerScoreInternal, false);
            NetworkServer.RegisterHandler<TeamScore>(OnServerTeamScoreInternal, false);

            base.RegisterServerMessages();
        }

        protected override void RegisterClientMessages() {
            Debug.LogFormat("RegisterClientMessages");
            NetworkClient.RegisterHandler<SubScenesMessage>(OnClientSubScenesInternal, false);
            
            base.RegisterClientMessages();
        }

        public void SetPlayerPrefab(GameObject prefab) {
            this.playerPrefab = prefab;
            if (playerPrefab != null) {
                ClientScene.RegisterPrefab(playerPrefab);
            }
        }
        public void AddSpawnPrefabs(List<GameObject> prefabs) {
            foreach(GameObject prefab in prefabs) {
                AddSpawnPrefab(prefab);
            }
        }

        public void AddSpawnPrefab(GameObject prefab) {
            if (prefab != null) {
                spawnPrefabs.Add(prefab);
                ClientScene.RegisterPrefab(prefab);
            }
        }

        internal struct SubScenesMessage : NetworkMessage {
            public List<string> subSceneNames;
            // Normal = 0, LoadAdditive = 1, UnloadAdditive = 2
            public SceneOperation sceneOperation;
            public bool customHandling;
        }

        public enum ScoreOperation : byte {
            Add,
            Subtract,
            Set
        }

        public enum TaskOperation : byte {
            Done,
            Undone,
            Other
        }

        internal struct TaskUpdate : NetworkMessage {
            public string id;
            // Add = 0, Subtract = 1, Set = 2
            public TaskOperation taskOperation;
            public bool customHandling;
        }

        internal struct PlayerScore : NetworkMessage {
            public int value;
            // Add = 0, Subtract = 1, Set = 2
            public ScoreOperation scoreOperation;
            public bool customHandling;
        }

        internal struct TeamScore : NetworkMessage {
            public int value;
            // Add = 0, Subtract = 1, Set = 2
            public ScoreOperation scoreOperation;
            public bool customHandling;
        }

        #region Server Internal Message Handlers


        private void OnTaskUpdateInternal(NetworkConnection conn, TaskUpdate msg) {
            logger.Log("NetworkController.OnServerTeamScoreInternal");

            OnServerTaskUpdateChange(conn, msg.id, msg.taskOperation, msg.customHandling);
        }

        private void OnServerPlayerScoreInternal(NetworkConnection conn, PlayerScore msg) {
            logger.Log("NetworkController.OnServerPlayerScoreInternal");

            OnServerPlayerScoreChange(conn, msg.value, msg.scoreOperation, msg.customHandling);
        }

        private void OnServerTeamScoreInternal(NetworkConnection conn, TeamScore msg) {
            logger.Log("NetworkController.OnServerTeamScoreInternal");

            OnServerTeamScoreChange(conn, msg.value, msg.scoreOperation, msg.customHandling);
        }

        #endregion


        #region Client Internal Message Handlers

        private void OnClientSubScenesInternal(NetworkConnection conn, SubScenesMessage msg) {
            logger.Log("NetworkController.OnClientSubScenesInternal");

            if (NetworkClient.isConnected && !NetworkServer.active) {
                ClientLoadSceneStack(msg.subSceneNames, msg.sceneOperation, msg.customHandling);
            }
        }

        

        #endregion

        #region Scene Management

        /// <summary>
        /// This causes the server to switch scenes and sets the networkSceneName.
        /// <para>Clients that connect to this server will automatically switch to this scene. This is called autmatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready during the change and ready again to participate in the new scene.</para>
        /// </summary>
        /// <param name="newSceneName"></param>
        public override void ServerChangeScene(string newSceneName) {
            if (string.IsNullOrEmpty(newSceneName)) {
                logger.LogError("ServerChangeScene empty scene name");
                return;
            }

            if (logger.logEnabled) logger.Log("ServerChangeScene " + newSceneName);
            NetworkServer.SetAllClientsNotReady();
            networkSceneName = newSceneName;

            // Let server prepare for scene change
            OnServerChangeScene(newSceneName);

            // Suspend the server's transport while changing scenes
            // It will be re-enabled in FinishScene.
            Transport.activeTransport.enabled = false;

            loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
            
            // ServerChangeScene can be called when stopping the server
            // when this happens the server is not active so does not need to tell clients about the change
            if (NetworkServer.active) {
                // notify all clients about the new scene
                NetworkServer.SendToAll(new SceneMessage { sceneName = newSceneName });
            }

            startPositionIndex = 0;
            startPositions.Clear();
        }

        /// <summary>
        /// This causes the server to switch scenes and sets the networkSceneName.
        /// <para>Clients that connect to this server will automatically switch to this scene. This is called autmatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready during the change and ready again to participate in the new scene.</para>
        /// </summary>
        /// <param name="newSceneName"></param>
        public void ClientsLoadScene(string newSceneName, SceneOperation sceneOperation = SceneOperation.Normal) {
            if (string.IsNullOrEmpty(newSceneName)) {
                logger.LogError("ClientsLoadSceneStack empty scene name");
                return;
            }

            if (logger.logEnabled) logger.Log("ClientsLoadSceneStack " + newSceneName + ", type: " + sceneOperation);
            NetworkServer.SetAllClientsNotReady();

            // ServerChangeScene can be called when stopping the server
            // when this happens the server is not active so does not need to tell clients about the change
            if (NetworkServer.active) {
                // notify all clients about the new scene
                NetworkServer.SendToAll(new SceneMessage { sceneName = newSceneName, sceneOperation = sceneOperation });
            }
        }

        /// <summary>
        /// This causes the server to switch scenes and sets the networkSceneName.
        /// <para>Clients that connect to this server will automatically switch to this scene. This is called autmatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready during the change and ready again to participate in the new scene.</para>
        /// </summary>
        /// <param name="subScenes"></param>
        public void ClientsLoadSceneStack(List<string> subScenes) {
            List<string> validScenes = subScenes.Where(x => !string.IsNullOrEmpty(x)).ToList();

            // ServerChangeScene can be called when stopping the server
            // when this happens the server is not active so does not need to tell clients about the change
            if (NetworkServer.active) {
                // notify all clients about the new scene
                NetworkServer.SendToAll(new SubScenesMessage { subSceneNames = validScenes, sceneOperation = SceneOperation.LoadAdditive });
            }
        }

        // This is only set in ClientChangeScene below...never on server.
        // We need to check this in OnClientSceneChanged called from FinishLoadSceneClientOnly
        // to prevent AddPlayer message after loading/unloading additive scenes
        internal void ClientLoadSceneStack(List<string> subScenes, SceneOperation sceneOperation = SceneOperation.LoadAdditive, bool customHandling = false) {
            Debug.LogFormat("ClientLoadSceneStack: {0}, {1}", subScenes, sceneOperation);

            sceneOperation = sceneOperation == SceneOperation.Normal ? SceneOperation.LoadAdditive : sceneOperation;

            if (logger.LogEnabled()) logger.Log("ClientLoadSceneStack subScenes:" + subScenes + " networkSceneName:" + networkSceneName);

            // vis2k: pause message handling while loading scene. otherwise we will process messages and then lose all
            // the state as soon as the load is finishing, causing all kinds of bugs because of missing state.
            // (client may be null after StopClient etc.)
            if (logger.LogEnabled()) logger.Log("ClientChangeScene: pausing handlers while scene is loading to avoid data loss after scene was loaded.");
            Transport.activeTransport.enabled = false;

            loadingSubScenesAsync = new List<AsyncOperation>();

            foreach (string scene in subScenes) {
                switch (sceneOperation) {
                    case SceneOperation.LoadAdditive:
                        // Ensure additive scene is not already loaded on client by name or path
                        // since we don't know which was passed in the Scene message
                        if (!SceneManager.GetSceneByName(scene).IsValid() && !SceneManager.GetSceneByPath(scene).IsValid())
                            loadingSubScenesAsync.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                        else {
                            logger.LogWarning($"Scene {scene} is already loaded");

                            // Re-enable the transport that we disabled before entering this switch
                            
                        }
                        break;
                    case SceneOperation.UnloadAdditive:
                        // Ensure additive scene is actually loaded on client by name or path
                        // since we don't know which was passed in the Scene message
                        if (SceneManager.GetSceneByName(scene).IsValid() || SceneManager.GetSceneByPath(scene).IsValid())
                            loadingSubScenesAsync.Add(SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects));
                        else {
                            logger.LogWarning($"Cannot unload {scene} with UnloadAdditive operation");

                            // Re-enable the transport that we disabled before entering this switch
                            
                        }
                        break;
                }
            }
            Transport.activeTransport.enabled = true;
        }

        static void UpdateSubscenes() {
            if (singleton != null && loadingSubScenesAsync != null && loadingSubScenesAsync.All(x => x.isDone)) {
                if (logger.LogEnabled()) logger.Log("ClientLoadSceneStack done readyCon:" + clientReadyConnection);
                // process queued messages that we received while loading the scene
                logger.Log("FinishLoadScene: resuming handlers after scene was loading.");
                Transport.activeTransport.enabled = true;
                loadingSubScenesAsync.ForEach(x => x.allowSceneActivation = true);
                loadingSubScenesAsync = null;
            }
            
        }

        #endregion

        #region Server System Callbacks

        /// <summary>
        /// Called on the server when a new client connects.
        /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerConnect(NetworkConnection conn) {
            Debug.LogFormat("Network Controller: Server Connect");
            conn.Send(new SceneMessage { sceneName = GameLobby, sceneOperation = SceneOperation.Normal });
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnection conn) {
            Debug.LogFormat("Network Controller: Server Disconnect");
            NetworkServer.DestroyPlayerForConnection(conn);
            PlayerManager.RemovePlayer(conn);
            logger.Log("OnServerDisconnect: Client disconnected.");
        }

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnection conn) {
            Debug.LogFormat("Network Controller: Server Ready");
            if (conn.identity == null) {
                // this is now allowed (was not for a while)
                logger.Log("Ready with no player object");
            }
            NetworkServer.SetClientReady(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn) {
            Debug.LogFormat("Network Controller: Server Add Player");
            NetworkServer.Spawn(NetworkingManager.NetworkDataController.gameObject, conn);
            if (autoCreatePlayer) {
                StartCoroutine(OnServerAddPlayerDelayed(conn));
            }
        }

        // This delay is mostly for the host player that loads too fast for the
        // server to have subscenes async loaded from OnStartServer ahead of it.
        IEnumerator OnServerAddPlayerDelayed(NetworkConnection conn) {
            // wait for server to async load all subscenes for game instances
            while (MySceneManager.loadingSubscenes)
                yield return null;

            conn.Send(new SceneMessage { sceneName = PlayerView, sceneOperation = SceneOperation.LoadAdditive });

            Transform startPos = GetStartPosition();
            GameObject playerObj = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);
            

            
            if (conn.identity != null) {
                NetworkServer.ReplacePlayerForConnection(conn, playerObj);
            } else {
                NetworkServer.AddPlayerForConnection(conn, playerObj);
            }

            while (conn.identity.GetComponent<PlayerController>() == null)
                yield return null;

            PlayerController playerController = conn.identity.GetComponent<PlayerController>();
            Player player = PlayerManager.NewPlayer(conn, 1);
            playerController.SetPlayer(player);

            //PlayerScore playerScore = conn.identity.GetComponent<PlayerScore>();
            //playerScore.playerNumber = clientIndex;
            //playerScore.scoreIndex = clientIndex / subScenes.Count;
            //playerScore.matchIndex = clientIndex % subScenes.Count;

            //clientIndex++;

            //if (subScenes.Count > 0)
            //    SceneManager.MoveGameObjectToScene(conn.identity.gameObject, subScenes[clientIndex % subScenes.Count]);
        }

        /// <summary>
        /// Called on the server when a network error occurs for a client connection.
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        /// <param name="errorCode">Error code.</param>
        public override void OnServerError(NetworkConnection conn, int errorCode) {
            Debug.LogFormat("Network Controller: Server Error");
        }

        /// <summary>
        /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
        /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
        /// </summary>
        /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
        public override void OnServerChangeScene(string newSceneName) {
            Debug.LogFormat("Network Controller: Server Scene Change");
        }

        /// <summary>
        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        /// </summary>
        /// <param name="sceneName">The name of the new scene.</param>
        public override void OnServerSceneChanged(string sceneName) {
            Debug.LogFormat("Network Controller: Server Scene Changed");
        }


        public void OnServerTaskUpdateChange(NetworkConnection conn, string id, TaskOperation taskOp = TaskOperation.Done, bool customHandling = false) {
            Debug.LogFormat("Network Controller: Server Team Score Change");
            Player player = PlayerManager.Player(conn);
            if(player.AssignedTasks.Any(x => x.ID == id)) {
                Task task = player.AssignedTasks.First(x => x.ID == id);
                switch (taskOp) {
                    case TaskOperation.Done:
                        task.CompleteRemote();
                        break;
                    case TaskOperation.Undone:
                        task.CompleteRemote(false);
                        break;
                    case TaskOperation.Other:
                        break;
                }
                PlayerManager.Players.Updated(player);
                PlayerManager.PlayerUpdated();
                TeamManager.Teams.Updated(player.Team);
                TeamManager.TeamUpdated();
            }

        }


        public void OnServerPlayerScoreChange(NetworkConnection conn, int value, ScoreOperation scoreOp = ScoreOperation.Add, bool customHandling = false) {
            Debug.LogFormat("Network Controller: Server Player Score Change");
            Player player = PlayerManager.Player(conn);
            switch (scoreOp) {
                case ScoreOperation.Add:
                    player.AddScore(value);
                    //PlayerManager.Players.AddOperation(SyncClassList<Player>.Operation.OP_SET, ownerInt, default, owner);
                    //TeamManager.Teams.AddOperation(SyncClassList<Team>.Operation.OP_SET, ownerTeamInt, default, owner.Team);
                    break;
                case ScoreOperation.Subtract:
                    player.AddScore(-value);
                    break;
                case ScoreOperation.Set:
                    player.SetScore(value);
                    break;
            }
            PlayerManager.Players.Updated(player);
            PlayerManager.PlayerUpdated();
        }

        public void OnServerTeamScoreChange(NetworkConnection conn, int value, ScoreOperation scoreOp = ScoreOperation.Add, bool customHandling = false) {
            Debug.LogFormat("Network Controller: Server Team Score Change");
            Player player = PlayerManager.Player(conn);
            switch (scoreOp) {
                case ScoreOperation.Add:
                    player.Team.AddScore(value);
                    break;
                case ScoreOperation.Subtract:
                    player.Team.AddScore(-value);
                    break;
                case ScoreOperation.Set:
                    player.Team.SetScore(value);
                    break;
            }
            TeamManager.Teams.Updated(player.Team);
            TeamManager.TeamUpdated();
        }

        #endregion

        #region Client System Callbacks

        /// <summary>
        /// Called on the client when connected to a server.
        /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientConnect(NetworkConnection conn) {
            // OnClientConnect by default calls AddPlayer but it should not do
            // that when we have online/offline scenes. so we need the
            // clientLoadedScene flag to prevent it.
            Debug.LogFormat("Network Controller: Client connected to server");
            if (!clientLoadedScene) {

                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                if (!ClientScene.ready) ClientScene.Ready(conn);
                
            }
            if(networkSceneName == GameLobby || networkSceneName == "NetworkLayer") {
                ClientScene.AddPlayer(conn);
            }
            NetworkingManager.Connected(conn);

        }

        /// <summary>
        /// Called on clients when disconnected from a server.
        /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientDisconnect(NetworkConnection conn) {
            Debug.LogFormat("Network Controller: Client Disconnected/Failed to connect to Server");
            StopClient();
            NetworkingManager.Disconnected(conn);
            NetworkingManager.NetworkDataController?.Destroy();
        }

        /// <summary>
        /// Called on clients when a network error occurs.
        /// </summary>
        /// <param name="conn">Connection to a server.</param>
        /// <param name="errorCode">Error code.</param>
        public override void OnClientError(NetworkConnection conn, int errorCode) {
            Debug.LogFormat("Network Controller: Client error: {0}", errorCode);
            NetworkingManager.Error(conn, errorCode);
            NetworkingManager.NetworkDataController?.Destroy();
        }

        /// <summary>
        /// Called on clients when a servers tells the client it is no longer ready.
        /// <para>This is commonly used when switching scenes.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientNotReady(NetworkConnection conn) {
            Debug.Log("Network Controller: Client - Server Not Ready");
        }

        /// <summary>
        /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
        /// </summary>
        /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
        /// <param name="sceneOperation">Scene operation that's about to happen</param>
        /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) {
            Debug.LogFormat("Network Controller: Client Scene Changing, {0}", newSceneName);
        }

        /// <summary>
        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
        /// </summary>
        /// <param name="conn">The network connection that the scene change message arrived on.</param>
        public override void OnClientSceneChanged(NetworkConnection conn) {
            Debug.Log("Network Controller: Client Scene Changed");         

            // always become ready.
            if (!ClientScene.ready) ClientScene.Ready(conn);

            Debug.LogFormat("{0},{1},{2}",clientSceneOperation, autoCreatePlayer, ClientScene.localPlayer);
            // Only call AddPlayer for normal scene changes, not additive load/unload
            if (clientSceneOperation == SceneOperation.Normal) {
                // add player if existing one is null
                ClientScene.AddPlayer(conn);
            }
        }

        #endregion

        #region Start & Stop callbacks

        // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
        // their functionality, users would need override all the versions. Instead these callbacks are invoked
        // from all versions, so users only need to implement this one case.

        /// <summary>
        /// This is invoked when a host is started.
        /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        public override void OnStartHost() {
            Debug.Log("Network Controller: Host Started");

        }

        /// <summary>
        /// This is invoked when a server is started - including when a host is started.
        /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
        /// </summary>
        public override void OnStartServer() {
            Debug.Log("Network Controller: Server Started");
            NetworkingManager.ServerStarted();
            // load all subscenes on the server only
            //StartCoroutine(LoadSubScenes());
        }

        /// <summary>
        /// This is invoked when the client is started.
        /// </summary>
        public override void OnStartClient() {
            Debug.Log("Network Controller: Client Started");
        }

        /// <summary>
        /// This is called when a host is stopped.
        /// </summary>
        public override void OnStopHost() {
            Debug.Log("Network Controller: Host Stopped");

        }

        /// <summary>
        /// This is called when a server is stopped - including when a host is stopped.
        /// </summary>
        public override void OnStopServer() {
            Debug.Log("Network Controller: Server Stopped");
            //NetworkServer.SendToAll(new SceneMessage { sceneName = GameScene, sceneOperation = SceneOperation.UnloadAdditive });
            //StartCoroutine(UnloadScenes());
            NetworkingManager.NetworkDataController?.Destroy();
        }

        /// <summary>
        /// This is called when a client is stopped.
        /// </summary>
        public override void OnStopClient() {
            Debug.Log("Network Controller: Client Stopped");
            //StartCoroutine(UnloadScenes());
            NetworkingManager.NetworkDataController?.Destroy();
        }

        #endregion
    }
}
