using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Game.Utility;
using UnityEngine.Networking;
using Game.Managers;

namespace Game.Tasks {
    [Serializable]
    public class StandardTask : Task {

        [JsonConstructor]
        public StandardTask(bool IsInProgress, bool IsCompleted, string ID, string Name, string Description, string Prefab, string Reward) : base(IsInProgress, IsCompleted, ID, Name, Description, Prefab, Reward) {
        }

        public StandardTask(string id, string name, string description, string prefab, string reward) : base(id, name, description, prefab, reward) {
        }

        public StandardTask(StandardTask task) : base(task) {
        }

        public override Task Clone() {
            return new StandardTask(this);
        }
    }
}