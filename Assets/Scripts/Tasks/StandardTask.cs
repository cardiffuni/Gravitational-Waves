using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Newtonsoft.Json;

namespace Game.Tasks {
    [Serializable]
    public class StandardTask : Task {
        [JsonConstructor]
        public StandardTask(string id, string name, string description, string prefab, string reward) : base(id, name, description, prefab, reward) {
        }

        public StandardTask(Task task) : base(task) {
        }

        public override Task Clone() {
            return new StandardTask(this);
        }
    }
}