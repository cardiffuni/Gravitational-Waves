using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using UnityEngine.Events;

namespace Game.Managers {
    public static class TeamManager {
      
        public static UnityEvent onTeamUpdate { get; private set; }

        public static List<Team> Teams { get; private set; }
        public static int NumTeams { get; private set; }
        public static int TimeLimit { get; private set; }
        public static bool TeamChat { get; private set; }

        public static bool Ready { get; private set; }

        static TeamManager() {
            Debug.Log("Loading TeamManager");
            Teams = new List<Team>();
            onTeamUpdate = new UnityEvent();
            SetDefaults();
            CreateTeams();
            Ready = true;
        }

        public static void Load() {
            
        }

        public static void CreateTeams() {
            Teams.Clear();
            for (int i = 0; i < NumTeams; i++) {
                Teams.Add(new Team(string.Format("Team{0}", i + 1), string.Format("Team {0}", i + 1),"XXXXXX"));
            }
            TeamUpdated();
        }

        internal static void CreateTeams(List<CodeJSON> data) {
            Teams.Clear();
            foreach (CodeJSON team in data) {
                Teams.Add(new Team(string.Format("Team{0}", team.team), string.Format("Team {0}", team.team),team.gen_code));
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
            TimeLimit = 60* val;
        }

        public static void SetTeamChat(bool val) {
            TeamChat = val;
        }

        internal static void SetDefaults() {
            SetTeamCount(SettingsManager.teamsDefaultNumber);
            SetTimeLimit(SettingsManager.timeMDefaultNumber);
            SetTeamChat(SettingsManager.teamChatDefault);
        }
    }
}