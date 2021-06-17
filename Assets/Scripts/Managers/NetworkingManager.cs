using Game.Managers.Controllers;
using Game.Network;
using Game.Teams;
using Game.Utility;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Game.Managers.NetworkingController;

namespace Game.Managers {

    public static class NetworkingManager {

        public static NetworkingController NetworkingController { get => InstanceManager.NetworkingController; }
        public static NetworkDataController NetworkDataController { get => NetworkDataController.Singleton; }

        public static bool isClient => NetworkingController.isClient;
        public static bool isServer => NetworkingController.isServer;
        public static bool isServerOnly => NetworkingController.isServerOnly;
        public static bool isHost => NetworkingController.isHost;

        public static NetworkIdentity LocalPlayer => ClientScene.localPlayer;
        public static NetworkConnection LocalConn => NetworkClient.connection;
        
        public static string ClientGenCode { get; private set; }
        public static string ClientHostIP { get; private set; }

        public static bool isClientConnected { get; private set; }
        public static bool isClientConnecting { get; private set; }

        public static bool isServerStarted { get; private set; }
        public static bool isServerStarting { get; private set; }


        public static UnityEvent OnClientConnecting { get; private set; }
        public static UnityEvent OnClientConnect { get; private set; }
        public static UnityEvent OnClientDisconnect { get; private set; }
        public static UnityEvent OnClientError { get; private set; }

        public static UnityEvent OnServerStarting { get; private set; }
        public static UnityEvent OnServerStarted { get; private set; }
        public static UnityEvent OnServerStopped { get; private set; }


        public static bool Ready { get; private set; }

        static NetworkingManager() {
            Debug.Log("Loading NetworkManager");
            OnClientConnecting = new UnityEvent();
            OnClientConnect = new UnityEvent();
            OnClientDisconnect = new UnityEvent();
            OnClientError = new UnityEvent();

            OnServerStarting = new UnityEvent();
            OnServerStarted = new UnityEvent();
            OnServerStopped = new UnityEvent();

            Ready = true;
        }

        private static string secretKey { get => SettingsManager.secretKey; } 

        public static void PreLoad() {
            NetworkingController.playerPrefab = AssetManager.Prefab("Player Character");
            RegisterPrefab("Network Data Controller");

        }

        public static void RegisterPrefab(string prefabID) {
            GameObject prefab = AssetManager.Prefab(prefabID);
            ClientScene.RegisterPrefab(prefab);
        }

        public static void StartServer() {
            isServerStarting = true;
            NetworkingController.StartServer();
            OnServerStarting?.Invoke();
        }

        public static void StartClient() {
            NetworkingController.networkAddress = ClientHostIP;
            //Uri uri = new Uri(ClientHostIP);
            //Debug.LogFormat("Connecting to: {0}", uri);
            //NetworkController.StartClient(uri);
            isClientConnecting = true;
            NetworkingController.StartClient();
            OnClientConnecting?.Invoke();
            //InstanceManager.InstanceController.StartCoroutine(GetIP(ClientGenCode));
        }

        internal static void GetFolderData<T>(string url, Action<Dictionary<string, T>> callback) where T : IServerData {
            InstanceManager.InstanceController.StartCoroutine(CoGetFolderData<T>(url, callback));
            
        }

        internal static void ClientsLoadScene(string scene) {
            foreach (NetworkConnection conn in NetworkServer.connections.Values) {
                conn.Send(new SceneMessage { sceneName = scene, sceneOperation = SceneOperation.Normal });
            }
            Debug.LogFormat("Team size: {0}", TeamManager.Teams.Count);
            foreach (Team team in TeamManager.Teams) {
                Debug.LogFormat("Team: Name {0}, Id {1}, Score {2}, GenCode {3}", team.Name, team.ID, team.Score, team.GenCode);
            }
            //Debug.LogFormat("Team2 size: {0}", TeamManager.Teams2.Count);
            //foreach (Team team in TeamManager.Teams2) {
            //    Debug.LogFormat("Team: Name {0}, Id {1}, Score {2}, GenCode {3}", team.Name, team.ID, team.Score, team.GenCode);
            //}
        }

        public static void CheckAndGetIP(string genCode, Action<bool, string> callback) {
            InstanceManager.InstanceController.StartCoroutine(CoGetIP(genCode, callback));
        }

        public static void AddToPlayerScore(int value) {
            LocalConn.Send(new PlayerScore { value = value, scoreOperation = ScoreOperation.Add });
        }

        public static void SubtractFromPlayerScore(int value) {
            LocalConn.Send(new PlayerScore { value = value, scoreOperation = ScoreOperation.Subtract });
        }

        public static void SetPlayerScore(int value) {
            LocalConn.Send(new PlayerScore { value = value, scoreOperation = ScoreOperation.Set });
        }

        public static void AddToTeamScore(int value) {
            LocalConn.Send(new TeamScore { value = value, scoreOperation = ScoreOperation.Add });
        }

        public static void SubtractFromTeamScore(int value) {
            LocalConn.Send(new TeamScore { value = value, scoreOperation = ScoreOperation.Subtract });
        }

        public static void SetTeamScore(int value) {
            LocalConn.Send(new TeamScore { value = value, scoreOperation = ScoreOperation.Set });
        }

        internal static void StopNetworking() {
            if (isServer) {
                NetworkingController.StopServer();
            }
            if (isClient) {
                NetworkingController.OnStopClient();
            }
        }

        public static void GetTeamGenCodes(Action<bool, string> callback) {
            InstanceManager.InstanceController.StartCoroutine(CoGetCodes(TeamManager.NumTeams, callback));
        }

        internal static void LoadGame() {
            NetworkingController.ServerChangeScene("NetworkLayer");
        }

        internal static void SetClientGenCode(string input) {
            Debug.LogFormat("GenCode '{0}' set", input);
            ClientGenCode = input;
        }

        internal static void SetClientHostIP(string input) {
            Debug.LogFormat("IP '{0}' set", input);
            ClientHostIP = input;
        }

        internal static IEnumerator LoadLobby() {
            while ( !isClientConnected && (NetworkDataController == null || !NetworkDataController.Ready)) { 
                Debug.LogFormat("NetworkDataController == null {0}", NetworkDataController == null); 
                 yield return null;
            }
            if (isServer) {
                TeamManager.CreateTeams();
            }
            ActionManager.LoadSceneLobby();
        }

        internal static void Connected(NetworkConnection conn) {
            Debug.LogFormat("Connected");
            //InstanceManager.StartNetworkDataController();
            isClientConnecting = false;
            isClientConnected = true;
            OnClientConnect?.Invoke();
            
        }

        internal static void Disconnected(NetworkConnection conn) {
            Debug.LogFormat("Disconnected");
            isClientConnected = false;
            isClientConnecting = false;
            OnClientDisconnect?.Invoke();
        }

        internal static void Error(NetworkConnection conn, int errorCode) {
            Debug.LogFormat("Error");
            isClientConnected = false;
            isClientConnecting = false;
            OnClientError?.Invoke();
        }

        internal static void ServerStarted() {
            Debug.LogFormat("Server Started");
            InstanceManager.StartNetworkDataController();
            Networking.UPnP();
            isServerStarted = true;
            isServerStarting = false;
            OnServerStarted?.Invoke();
        }

        internal static void ServerStopped() {
            Debug.LogFormat("Server Stopped");
            isServerStarted = false;
            isServerStarting = false;
            OnServerStopped?.Invoke();
        }

        // remember to use StartCoroutine when calling this function!
        private static IEnumerator CoGetCodes(int teams, Action<bool, string> callback) {
            //This connects to a server side php script that will add the name and score to a MySQL DB.
            // Supply it with a string representing the players name and the players score.
            string ip = null;
            yield return Networking.CheckIP(result => { ip = result; });
            if (ip != null) {
                Debug.LogFormat("Your IP: {0}", ip);

                string hash = Cryptography.Md5Sum(ip + teams + secretKey);
                Debug.LogFormat("sending hash: {0}", hash);

                WWWForm form = new WWWForm();
                form.AddField("host_ip", ip);
                form.AddField("teams", teams);
                form.AddField("hash", hash);

                yield return Networking.PostRequest(SettingsManager.codesURL, form, (Action<UnityEngine.Networking.UnityWebRequest>)(webRequest => {
                    string text = webRequest.downloadHandler.text;
                    ServerRequest<CodeJSON> json = Functions.StringJsonToObject<ServerRequest<CodeJSON>>((string)text);
                    if(json != null && json.data != null && json.data.Count > 0) {
                        string checkhash = Cryptography.Md5Sum(json.timestamp + secretKey);
                        Debug.LogFormat("Received: {0}", json);
                        if (checkhash == json.hash) {
                            TeamManager.StoreTeamCodes(json.data);
                            callback(true, "Code retreived");
                        } else {
                            Debug.LogErrorFormat("Received Hash {0} doesn't match calcalated hash {1} ", json.hash, checkhash);
                            callback(false, "Communcation Error 04");
                        }
                    } else {
                        Debug.LogErrorFormat("Invalid reponce: {0}", text);
                        callback(false, "Communcation Error 03");
                    }
                }));

            } else {
                Debug.LogFormat("Could not find external IP");
                callback(false, "Communcation Error 06");
            }
        }

        // Get the scores from the MySQL DB to display in a GUIText.
        // remember to use StartCoroutine when calling this function!
        private static IEnumerator CoGetIP(string genCode, Action<bool,string> callback) {
            string hash = Cryptography.Md5Sum(genCode + secretKey);
            Debug.LogFormat("sending hash: {0}", hash);

            WWWForm form = new WWWForm();
            form.AddField("gen_code", genCode);
            form.AddField("hash", hash);

            yield return Networking.PostRequest(SettingsManager.ipURL, form, (Action<UnityEngine.Networking.UnityWebRequest>)(webRequest => {
                string text = webRequest.downloadHandler.text;
                ServerRequest<CodeJSON> json = Functions.StringJsonToObject<ServerRequest<CodeJSON>>((string)text);
                if (json != null && json.data != null) {
                    string checkhash = Cryptography.Md5Sum(json.timestamp + secretKey);
                    if (checkhash == json.hash) {
                        if(json.data.Count > 0) {
                            foreach (KeyValuePair<string, CodeJSON> item in json.data) {
                                CodeJSON code = item.Value;
                                Debug.LogFormat("Host IP for code {0} is {1}.", genCode, code.host_ip);
                                callback(true, code.host_ip);
                            }
                        } else {
                            callback(false, "Invalid Code");
                        }
                        
                    } else {
                        Debug.LogErrorFormat("Received Hash {0} doesn't match calcalated hash {1} ", json.hash, checkhash);
                        callback(false, "Communcation Error 04");
                    }
                } else {
                    Debug.LogErrorFormat("Invalid reponce: {0}", text);
                    callback(false, "Communcation Error 03");
                }
            }));
        }

        private static IEnumerator CoGetFolderData<T>(string url, Action<Dictionary<string, T>> callback) where T : IServerData {

            yield return Networking.GetRequest(url, (Action<UnityEngine.Networking.UnityWebRequest>)(webRequest => {
                string text = webRequest.downloadHandler.text;
                ServerRequest<T> json = Functions.StringJsonToObject<ServerRequest<T>>((string)text);
                if (json != null && json.data != null) {
                    string checkhash = Cryptography.Md5Sum(json.timestamp + secretKey);
                    if (checkhash == json.hash) {
                        if (json.data.Count > 0) {
                            if (json.data != null) {
                                callback(json.data);
                            } else {
                                Debug.LogErrorFormat("Empty data: {0}", json);
                            }
                            
                        } else {
                            Debug.LogErrorFormat("Empty JSON: {0}", json);
                        }

                    } else {
                        Debug.LogErrorFormat("Received Hash {0} doesn't match calcalated hash {1} ", json.hash, checkhash);
                    }
                } else {
                    Debug.LogErrorFormat("Invalid reponce: {0}", text);
                }
            }));
        }

        public static void Load() { }
    }
}