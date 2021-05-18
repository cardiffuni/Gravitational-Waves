using System;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using Game.Utility;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game.Managers {
    [RequireComponent(typeof(KcpTransport))]
    [RequireComponent(typeof(NetworkManagerHUD))]
    public class NetworkController : Mirror.NetworkManager {

        public KcpTransport KcpTransport { get; private set; }

        public override void Awake() {
            dontDestroyOnLoad = false;
            autoCreatePlayer = false;
            KcpTransport = gameObject.GetOrAddComponent<KcpTransport>();
            transport = KcpTransport;
            showDebugMessages = true;
            base.Awake();
        }
        /// <summary>
        /// This starts a network "host" - a server and client in the same application.
        /// <para>The client returned from StartHost() is a special "local" client that communicates to the in-process server using a message queue instead of the real network. But in almost all other cases, it can be treated as a normal client.</para>
        /// </summary>
        public new void StartHost() {
            //Debug.Log(transport);
            //Debug.Log(Transport.activeTransport);
            Networking.UPnP();
            base.StartHost();
            
        }
        public override void OnClientConnect(NetworkConnection conn) {
            // OnClientConnect by default calls AddPlayer but it should not do
            // that when we have online/offline scenes. so we need the
            // clientLoadedScene flag to prevent it.
            Debug.LogFormat("Client connected to server");
            if (!clientLoadedScene) {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                if (!ClientScene.ready) ClientScene.Ready(conn);
                if (autoCreatePlayer) {
                    ClientScene.AddPlayer(conn);
                }
            }
            NetworkManager.Connected(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            Debug.LogFormat("Client disconnected to server");
            StopClient();
            NetworkManager.Disconnected(conn);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode) {
            Debug.LogFormat("Client error: {0}", errorCode);
            NetworkManager.Error(conn, errorCode);
        }

        public override void OnStartServer() {
            
            Debug.LogFormat("My Server Started: {0}", NetworkServer.dontListen);
            Debug.Log("My Server Started");
        }
    }

}
