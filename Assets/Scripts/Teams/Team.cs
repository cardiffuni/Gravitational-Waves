using UnityEngine;
using System.Collections;
using System;
using Mirror;
using Game.Managers;

namespace Game.Teams {
    [Serializable]
    public class Team {

        [SerializeField]
        public string Name { get; protected set; }
        [SerializeField]
        public string ID { get; protected set; }
        [SerializeField]
        public string GenCode { get; protected set; }
        [SerializeField]
        public int Score { get; protected set; }

        public Team(string Name, string ID, string GenCode) {
            this.Name = Name;
            this.ID = ID;
            this.GenCode = GenCode;
            this.Score = 0;
        }
        public Team() {}

        internal void AddScore(int value) {
            Score += value;
            TeamManager.TeamUpdated(this);
        }

        internal void SetScore(int value) {
            Score = value;
            TeamManager.TeamUpdated(this);
        }
        public string GetDescription() {
            return string.Format("Score: {0}", Score);
        }
    }
}