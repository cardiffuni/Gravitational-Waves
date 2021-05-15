using UnityEngine;
using System.Collections;
using System;

namespace Game.Teams {

    public class Team {
        public string Name { get; protected set; }
        public string Id { get; protected set; }

        public string GenCode { get; protected set; }
        public int Score { get; protected set; }

        public Team(string id, string name, string genCode) {
            Name = name;
            Id = id;
            GenCode = genCode;
            Score = 0;
        }

        internal void AddScore(int value) {
            Score += value;
        }

        public string GetDescription() {
            return string.Format("Score: {0}", Score);
        }
    }
}