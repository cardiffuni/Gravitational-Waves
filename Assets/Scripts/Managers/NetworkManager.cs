using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using Mirror;
using Game.Network;
using Game.Utility;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Game.Managers {


    internal class ServerJSON {
        public string msg { get; set; }
        public string result { get; set; }
        public string error { get; set; }
        public int timestamp { get; set; }
        public List<CodeJSON> data { get; set; }
        public string permitted_chars { get; set; }
        public string hash { get; set; }
    }
    internal class CodeJSON {
        public bool worked { get; set; }
        public int trys { get; set; }
        public string gen_code { get; set; }
        public string host_ip { get; set; }
        public int team { get; set; }
    }

    public static class NetworkManager {

        public static NetworkController NetworkController { get => InstanceManager.NetworkController; }

        public static string ClientGenCode { get; private set; }

        public static bool Ready { get; private set; }

        static NetworkManager() {
            Debug.Log("Loading NetworkManager");
            Ready = true;
        }
 
        private static string secretKey = "poopoopoopoo"; // Edit this value and make sure it's the same as the one stored on the server
        public static string getCodesURL = "https://gravwaves.azurewebsites.net/getcodes.php"; //be sure to add a ? to your url
        public static string getIPURL = "https://gravwaves.azurewebsites.net/getip.php";

        public static void StartHost() {
            NetworkController.StartHost();
            InstanceManager.InstanceController.StartCoroutine(GetCodes(TeamManager.NumTeams));
        }

        public static void StartClient() {
            NetworkController.StartClient();
            InstanceManager.InstanceController.StartCoroutine(GetIP(ClientGenCode));
        }

        internal static void SetClientGenCode(string inputFormatted) {
            ClientGenCode = inputFormatted;
        }

        // remember to use StartCoroutine when calling this function!
        public static IEnumerator GetCodes(int teams) {


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

                yield return Networking.PostRequest(getCodesURL, form, webRequest => {
                    string text = webRequest.downloadHandler.text;
                    ServerJSON json = Functions.StringToJson<ServerJSON>(text);
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
        public static IEnumerator GetIP(string genCode) {
            string hash = Cryptography.Md5Sum(genCode + secretKey);
            Debug.LogFormat("sending hash: {0}", hash);

            WWWForm form = new WWWForm();
            form.AddField("gen_code", genCode);
            form.AddField("hash", hash);

            yield return Networking.PostRequest(getIPURL, form, webRequest => {
                string text = webRequest.downloadHandler.text;
                ServerJSON json = Functions.StringToJson<ServerJSON>(text);
                if (json != null && json.data != null && json.data.Count > 0) {
                    string checkhash = Cryptography.Md5Sum(json.timestamp + secretKey);
                    if (checkhash == json.hash) {
                        foreach (CodeJSON code in json.data) {
                            Debug.Log(code.host_ip);
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