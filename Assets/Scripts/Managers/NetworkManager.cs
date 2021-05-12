using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using Mirror;

namespace Game.Managers {
    public static class NetworkManager {

        public static Mirror.NetworkManager NetworkController { get => InstanceManager.NetworkController; }

        public static bool Ready { get; private set; }

        static NetworkManager() {
            Debug.Log("Loading NetworkManager");
            Ready = true;
        }

        public static void Load() { }
    }
}