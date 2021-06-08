using Game.Managers;
using Game.Network;
using Game.Players;
using Game.Scores;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tasks {
    [Serializable]
    public abstract class Task : ITask, IServerData {
        public bool IsInProgress { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public string ID { get; protected set; }
        
        public string Name { get; protected set; }
        public string Title { get => Name; }

        public string Description { get; protected set; }
        public string Prefab { get; protected set; }
        public string Reward { get; protected set; }
        public Task Parent { get; protected set; }
        public Player Owner { get; protected set; }

        public Task(bool IsInProgress, bool IsCompleted, string ID, string Name, string Description, string Prefab, string Reward) {
            this.IsInProgress = IsInProgress;
            this.IsCompleted = IsCompleted;
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Prefab = Prefab;
            this.Reward = Reward;
            this.Parent = TaskManager.Task(ID);
        }
        

        public Task(string id, string name, string description, string prefab, string reward) {
            ID = id;
            Name = name;
            Description = description;
            Prefab = prefab;
            Reward = reward;
            defaults();
        }

        public Task(Task task) {
            Parent = task;
            ID = task.ID;
            Name = task.Name;
            Description = task.Description;
            Prefab = task.Prefab;
            Reward = task.Reward;
            defaults();
        }

        private void defaults() {
            IsInProgress = false;
            IsCompleted = false;
        }

        public abstract Task Clone();

        public void Complete(bool value = true) {
            IsCompleted = value;
            if (IsCompleted) {
                Debug.LogFormat("Task {0} Completed!", Name);
                IsInProgress = false;
                ScoreManager.AddScore(Owner, Reward);
                TaskManager.TaskUpdated();
            } else {
                Debug.LogFormat("Task {0} reset", Name);
            }
        }

        internal void ToggleCompleted() {
            Complete(!IsCompleted);
        }

        internal void Started(bool value = true) {
            if (value) {
                Debug.LogFormat("Task {0} started!", Name);
            } else {
                Debug.LogFormat("Task {0} no longer started", Name);
            }
            IsInProgress = value;
        }

        public string GetID() => ID;

        public string GetDescription() => Description;

        public string GetTitle() => Name;

        public void SetOwner(Player player) {
            Owner = player;
        }

        public Task GetOrigin() {
            return Parent == null ? this : Parent.GetOrigin();
        }
    }
}
