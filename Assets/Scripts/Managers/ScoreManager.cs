using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Game.Teams;
using Game.Players;
using Game.Tasks;
using Game.Scores;
using System.Linq;
using Game.Network;

namespace Game.Managers {
    public static class ScoreManager {

        public static List<Reward> Rewards { get; private set; }

        public static bool Ready { get; private set; }


        static ScoreManager() {
            Debug.Log("Loading ScoreManager");
            Rewards = new List<Reward>() {
                new Reward("Reward1", 50, 50),
                new Reward("Reward2", 100, 100),
                new Reward("Reward3", 150, 150),
                new Reward("Reward4", 200, 200),
            };
            Ready = true;
        }

        public static void Load() { }

        public static void AddScore(Player owner, string rewardID) {

            Reward reward = Reward(rewardID);

            owner.AddScore(reward.PlayerAmount);
            owner.Team.AddScore(reward.TeamAmount);
        }

        public static Reward Reward(string id) {
            if (Rewards.Any(x => x.Id.ToLower() == id.ToLower())) {
                return Rewards.First(x => x.Id.ToLower() == id.ToLower());
            } else {
                Debug.LogWarningFormat("No Score Found Named: {0}", id);
                return null;
            }

        }
    }
}