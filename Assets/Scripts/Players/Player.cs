using UnityEngine;
using System.Collections;
using System;
using System.Net;
using Game.Tasks;
using System.Collections.Generic;
using Game.Managers;
using System.Linq;
using Mirror;
using Game.Teams;
using Newtonsoft.Json;

namespace Game.Players {
    public class Player {
        public string Name { get; protected set; }
        public bool IsHost { get; protected set; }
        public int ID { get; protected set; }
        public int ConnId { get; protected set; }

        public List<Task> AssignedTasks { get; protected set; }
        public string Thumbnail { get; protected set; }

        public int NumberOfTasks { get; protected set; }
        public Team Team { get; protected set; }

        public int Score { get; protected set; }

        public static int PlayerIndex = 0;

        public Player() {
            Name = "Default Player";
            NumberOfTasks = SettingsManager.taskDefaultNumber;
            Score = 0;
        }

        public Player(string name, Team team, int connId) {
            Name = name;
            Thumbnail = "gravitationalwaves-tex";
            AssignedTasks = new List<Task>();
            IsHost = false;
            PlayerIndex++;
            ID = PlayerIndex;
            ConnId = connId;

            NumberOfTasks = SettingsManager.taskDefaultNumber;
            PlayerManager.AssignRandomTasks(this);
            Team = team;
            Score = 0;
        }

        [JsonConstructor]
        public Player(string Name, bool IsHost, int ConnId, int ID, List<Task> AssignedTasks, string Thumbnail, int NumberOfTasks, Team Team, int Score) {
            this.Name = Name;
            this.Thumbnail = Thumbnail;
            Debug.LogFormat("AssignedTasks.Count: {0}", AssignedTasks.Count);
            this.AssignedTasks = AssignedTasks;
            AssignedTasks.ForEach(x => x.SetOwner(this));
            this.IsHost = IsHost;

            this.ID = ID;
            this.ConnId = ConnId;

            this.NumberOfTasks = NumberOfTasks;
            this.Team = Team;
            this.Score = Score;
        }

        internal void AddScore(int value) {
            Score += value;
        }
        internal void SetScore(int value) {
            Score = value;
        }
        public void SetName(string value) {
            Name = value;
        }

        public string GetDescription() {
            return string.Format("Score: {0}", Score);
        }

        public void SetIsHost(bool value) {
            IsHost = value;
        }
    
        public void AssignTask(Task task) {
            task.SetOwner(this);
            AssignedTasks.Add(task);
        }

        public float PercentageTasksComplete() {
            int tasksComplete = AssignedTasks.Count(x => x.IsCompleted);
            float percentage = ((float)tasksComplete / AssignedTasks.Count()) * 100;
            Debug.LogFormat("Task Completed: {0}/{1} - {2}%", tasksComplete, AssignedTasks.Count(), percentage);
            return percentage;
        }
    }
}