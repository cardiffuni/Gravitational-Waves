using UnityEngine;
using System.Collections;
using System;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using Mirror;
using Game.Network;
using Game.Utility;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Managers {

    public static class NetworkManager {

        public static NetworkController NetworkController { get => InstanceManager.NetworkController; }

        public static string ClientGenCode { get; private set; }
        public static string ClientHostIP { get; private set; }
        public static bool ClientConnected { get; private set; }
        public static bool ClientConnecting { get; private set; }

        public static UnityEvent OnClientConnecting { get; private set; }
        public static UnityEvent OnClientConnect { get; private set; }
        public static UnityEvent OnClientDisconnect { get; private set; }
        public static UnityEvent OnClientError { get; private set; }


        public static bool Ready { get; private set; }

        static NetworkManager() {
            Debug.Log("Loading NetworkManager");
            OnClientConnecting = new UnityEvent();
            OnClientConnect = new UnityEvent();
            OnClientDisconnect = new UnityEvent();
            OnClientError = new UnityEvent();

            Ready = true;
        }

        private static string secretKey { get => SettingsManager.secretKey; } 

        public static void StartHost() {
            NetworkController.StartHost();
            InstanceManager.InstanceController.StartCoroutine(CoGetCodes(TeamManager.NumTeams));
        }

        public static void StartClient() {
            NetworkController.networkAddress = ClientHostIP;
            //Uri uri = new Uri(ClientHostIP);
            //Debug.LogFormat("Connecting to: {0}", uri);
            //NetworkController.StartClient(uri);
            ClientConnecting = true;
            NetworkController.StartClient();
            OnClientConnecting?.Invoke();

            //InstanceManager.InstanceController.StartCoroutine(GetIP(ClientGenCode));
        }

        internal static void GetFolderData<T>(string url, Action<Dictionary<string, T>> callback) where T : IServerData {
            InstanceManager.InstanceController.StartCoroutine(CoGetFolderData<T>(url, callback));
            
        }

        public static void CheckAndGetIP(string genCode, Action<bool, string> callback) {
            InstanceManager.InstanceController.StartCoroutine(CoGetIP(genCode, callback));
    }

        internal static void SetClientGenCode(string input) {
            Debug.LogFormat("GenCode '{0}' set", input);
            ClientGenCode = input;
        }

        internal static void SetClientHostIP(string input) {
            Debug.LogFormat("IP '{0}' set", input);
            ClientHostIP = input;
        }

        internal static void Connected(NetworkConnection conn) {
            Debug.LogFormat("Connected");
            ClientConnected = true;
            OnClientConnect?.Invoke();
            ActionManager.LoadSceneLobby();
        }

        internal static void Disconnected(NetworkConnection conn) {
            Debug.LogFormat("Disconnected");
            ClientConnected = false;
            if (ClientConnecting) {
                ClientConnecting = false;
                OnClientDisconnect?.Invoke();
            }
        }

        internal static void Error(NetworkConnection conn, int errorCode) {
            Debug.LogFormat("Error");
            ClientConnected = false;
            if (ClientConnecting) {
                ClientConnecting = false;
                OnClientError?.Invoke();
            }
        }

        // remember to use StartCoroutine when calling this function!
        private static IEnumerator CoGetCodes(int teams) {
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

                yield return Networking.PostRequest(SettingsManager.codesURL, form, webRequest => {
                    string text = webRequest.downloadHandler.text;
                    ServerRequest<CodeJSON> json = Functions.StringToJson<ServerRequest<CodeJSON>>(text);
                    if(json != null && json.data != null && json.data.Count > 0) {
                        string checkhash = Cryptography.Md5Sum(json.timestamp + secretKey);
                        Debug.LogFormat("Received: {0}",json);
                        if (checkhash == json.hash) {
                            TeamManager.CreateTeams(json.data);
                        } else {
                            Debug.LogErrorFormat("Received Hash {0} doesn't match calcalated hash {1} ", json.hash, checkhash);
                        }
                    } else {
                        Debug.LogErrorFormat("Invalid reponce: {0}", text);
                    }
                });

            } else {
                Debug.LogFormat("Could not find external IP");
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

            yield return Networking.PostRequest(SettingsManager.ipURL, form, webRequest => {
                string text = webRequest.downloadHandler.text;
                ServerRequest<CodeJSON> json = Functions.StringToJson<ServerRequest<CodeJSON>>(text);
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
            });
        }

        private static IEnumerator CoGetFolderData<T>(string url, Action<Dictionary<string, T>> callback) where T : IServerData {

            yield return Networking.GetRequest(url, webRequest => {
                string text = webRequest.downloadHandler.text;
                ServerRequest<T> json = Functions.StringToJson<ServerRequest<T>>(text);
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
            });
        }

        public static void Load() { }
    }
}