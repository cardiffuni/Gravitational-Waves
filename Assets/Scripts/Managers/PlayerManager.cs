using UnityEngine;
using System.Collections;
using System;
using Game.Players;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using Game.Utility;

namespace Game.Managers {
    public static class PlayerManager {

        public static bool Ready { get; private set; }

        public static UnityEvent onPlayerUpdate { get; private set; }
        public static Player LocalPlayer { get; private set; }

        public static List<string> FirstNames { get; private set; }
        public static List<string> MiddleNames { get; private set; }
        public static List<string> LastNames { get; private set; }

        static PlayerManager() {
            Debug.Log("Loading PlayerManager");
            LocalPlayer = new Player("Test Player", TeamManager.Teams.First());
            onPlayerUpdate = new UnityEvent();
            Dictionary<string, List<string>>  randomNames = AssetManager.JSON<Dictionary<string, List<string>>>("random_names");
            FirstNames = randomNames["First"];
            MiddleNames = randomNames["Middle"];
            LastNames = randomNames["Last"];
            Debug.LogFormat("Names:\nFirst:{0}\n Middle{1}\nLast:{2}", FirstNames, MiddleNames, LastNames);
            Ready = true;
        }

        public static void Load() { }

        public static void AssignRandomTasks(Player player) {
            for (int i = 0; i < player.NumberOfTasks; i++) {
                player.AssignTask(TaskManager.GetRandomTask().Clone());
            }
        }

        public static void AddPlayerUpdateListener(UnityAction action) {
            onPlayerUpdate.AddListener(action);
        }

        public static void RemovePlayerUpdateListener(UnityAction action) {
            onPlayerUpdate.RemoveListener(action);
        }

        public static void PlayerUpdated() {
            onPlayerUpdate?.Invoke();
        } 
    }
}