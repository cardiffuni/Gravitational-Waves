using System;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game.Network
{
    [RequireComponent(typeof(KcpTransport))]
    public class NetworkController : Mirror.NetworkManager {

        public KcpTransport KcpTransport { get; private set; }

        public override void Awake() {
            dontDestroyOnLoad = false;
            autoCreatePlayer = false;
            KcpTransport = gameObject.GetOrAddComponent<KcpTransport>();
            transport = KcpTransport;
            base.Awake();
        }
        /// <summary>
        /// This starts a network "host" - a server and client in the same application.
        /// <para>The client returned from StartHost() is a special "local" client that communicates to the in-process server using a message queue instead of the real network. But in almost all other cases, it can be treated as a normal client.</para>
        /// </summary>
        public new void StartHost() {
            Debug.Log(transport);
            Debug.Log(Transport.activeTransport);
            //LogFilter.Debug = true;
            //LogFactory.EnableDebugMode();
            
            base.StartHost();
            
        }

    }

}
