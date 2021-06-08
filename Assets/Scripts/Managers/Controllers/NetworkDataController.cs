using Game.Network;
using Game.Players;
using Game.Teams;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Managers.Controllers {
    public class NetworkDataController : NetworkBehaviour {
        public SyncClassList<Team> Teams = new SyncClassList<Team>();
        public SyncClassList<Player> Players = new SyncClassList<Player>();

        public static NetworkDataController Singleton;

        public bool Ready { get; private set; }

        private void OnEnable() {

        }

        // Start is called before the first frame update
        void Start() {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            Ready = true;
        }

        private void OnConnectedToServer() {

        }

        private void OnServerInitialized() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}