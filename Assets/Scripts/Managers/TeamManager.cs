using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using UnityEngine.Events;
using Game.Network;
using Mirror;

namespace Game.Managers {
    public static class TeamManager {

        public static UnityEvent onTeamUpdate { get; private set; }

        public static SyncClassList<Team> Teams { get => NetworkingManager.NetworkDataController.Teams; set => NetworkingManager.NetworkDataController.Teams = value; }
        private static Dictionary<string, CodeJSON> teamCodes;


        public static int NumTeams { get; private set; }
        public static int TimeLimit { get; private set; }
        public static bool TeamChat { get; private set; }

        public static bool Ready { get; private set; }

        static TeamManager() {
            Debug.Log("Loading TeamManager");
            
            onTeamUpdate = new UnityEvent();
            SetDefaults();
            Ready = true;
        }

        public static void Load() {

        }

        internal static void CreateTeams() {
            Teams.Reset();
            foreach (KeyValuePair<string, CodeJSON> item in teamCodes) {
                CodeJSON team = item.Value;
                Teams.Add(new Team(string.Format("Team{0}", team.team), string.Format("Team {0}", team.team), team.gen_code));
            }
            TeamUpdated();
        }

        public static void AddTeamUpdateListener(UnityAction action) {
            onTeamUpdate.AddListener(action);
        }

        public static void RemoveTeamUpdateListener(UnityAction action) {
            onTeamUpdate.RemoveListener(action);
        }

        public static void TeamUpdated() {
            onTeamUpdate?.Invoke();
        }

        public static void SetTeamCount(int val) {
            NumTeams = val;
        }

        public static void SetTimeLimit(int val) {
            TimeLimit = 60 * val;
        }

        public static void SetTeamChat(bool val) {
            TeamChat = val;
        }

        internal static void SetDefaults() {
            SetTeamCount(SettingsManager.teamsDefaultNumber);
            SetTimeLimit(SettingsManager.timeMDefaultNumber);
            SetTeamChat(SettingsManager.teamChatDefault);
        }

        internal static void StoreTeamCodes(Dictionary<string, CodeJSON> data) {
            teamCodes = data;
        }
    }
}