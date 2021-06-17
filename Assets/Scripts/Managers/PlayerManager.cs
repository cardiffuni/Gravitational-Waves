using UnityEngine;
using System.Collections;
using System;
using Game.Players;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using Game.Utility;
using Game.Network;
using Mirror;
using Game.Teams;
using Game.Managers.Controllers;

namespace Game.Managers {
    public static class PlayerManager {

        public static bool Ready { get; private set; }

        public static UnityEvent onPlayerUpdate { get; private set; }

        public static SyncClassList<Player> Players { get => NetworkingManager.NetworkDataController?.Players; set => NetworkingManager.NetworkDataController.Players = value; }

        public static int LocalPlayerID { get => Convert.ToInt32(NetworkingManager.LocalConn?.identity?.GetComponent<PlayerController>().PlayerID); }

        public static Player LocalPlayer { get => GetLocalPlayer(); }

        public static List<string> FirstNames { get; private set; }
        public static List<string> MiddleNames { get; private set; }
        public static List<string> LastNames { get; private set; }

        static PlayerManager() {
        }

        public static Player NewPlayer(NetworkConnection conn, int teamNum) {
            Player player = Player(conn);
            if (player != null) {
                return player;
            } else {
                Team team = TeamManager.Teams[teamNum - 1];
                Debug.LogFormat("New Player, Team: {0}, ConnId: {1}", team, conn.connectionId);
                player = new Player("Test Player", team, conn.connectionId);
                Players.Add(player);
                return player;
            }
        }

        public static void RemovePlayer(NetworkConnection conn) {
            Player player = Player(conn);
            if (player != null) {
                Players.Remove(player);
            }
        }

        public static void Load() {
            Debug.Log("Loading PlayerManager");
            //LocalPlayer = new Player("Test Player", TeamManager.Teams.First());
            onPlayerUpdate = new UnityEvent();
            Dictionary<string, List<string>> randomNames = AssetManager.JSON<Dictionary<string, List<string>>>("random_names");
            FirstNames = randomNames["First"];
            MiddleNames = randomNames["Middle"];
            LastNames = randomNames["Last"];
            Debug.LogFormat("Names:\nFirst:{0}\n Middle{1}\nLast:{2}", FirstNames, MiddleNames, LastNames);
            Ready = true;
        }

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
            Debug.LogFormat("Player Updated");
            onPlayerUpdate?.Invoke();
        }
        public static void PlayerUpdated(SyncClassList<Player>.Operation op, int itemIndex, Player oldItem, Player newItem) {
            Debug.LogFormat("Player {0} Updated", newItem.Name);
            PlayerUpdated();
        }
        internal static Player GetLocalPlayer() {
            //Debug.LogFormat("PlayerController: {0}", NetworkingManager.LocalConn?.identity?.GetComponent<PlayerController>());

            if (Players != null && LocalPlayerID > 0) {
                //Debug.LogFormat("Players count: {0}", Players.Count());
                if (Players.Count() > 0) {
                    //Debug.LogFormat("Players[0] id: {0}", Players[0].ID);
                    Player player = Players.DefaultIfEmpty(null).FirstOrDefault(x => x.ID == LocalPlayerID);
                    Debug.LogFormat("Player: {0}", player.Name);
                    return player;
                }
            }
            return null;
        }

        internal static Player Player(NetworkConnection conn) {
            int connId = conn.connectionId;
            if (Players.Any(x => x.ConnId == connId)) {
                return Players.First(x => x.ConnId == connId);
            } else {
                return null;
            }
        }
    }
}