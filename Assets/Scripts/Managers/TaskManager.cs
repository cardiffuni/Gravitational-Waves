using UnityEngine;
using System.Collections;
using System;
using Game.Tasks;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Game.Teams;
using Game.Utility;

namespace Game.Managers {
    public static class TaskManager {

        public static List<Task> Tasks { get; private set; }
        public static Task GetRandomTask() => Tasks[UnityEngine.Random.Range(0, Tasks.Count)];
        public static UnityEvent onTaskUpdate { get; private set; }

        public static bool Ready { get; private set; }


        static TaskManager() {
            Debug.Log("Loading TaskManager");
            Tasks = new List<Task>() {
                new StandardTask("MirrorCleaning", "Mirror Cleaning", "Clean those mirrors, fool!", "Mirror Cleaning Task", "Reward1"),
                new StandardTask("WaveformFitter", "Waveform Fitter", "Fit da Waveform", "Waveform Fitter Task", "Reward2"),
                new StandardTask("Range", "Range", "Get the waveform in the correct area?", "Range Task", "Reward3"),
                new StandardTask("Laserpointer", "Laser pointer", "Point da lazor to the right place!", "Mirror Cleaning Task", "Reward2"),
                new StandardTask("EasterCrow", "Easter Crow", "Chirp Chirp", "Mirror Cleaning Task", "Reward3"),
                new StandardTask("HelloWorld", "Hello World", "Say Hi!", "Mirror Cleaning Task", "Reward4"),
            };
            //Functions.ExportToJson(Tasks);
            onTaskUpdate = new UnityEvent();
            Ready = true;
        }

        internal static void TriggeredTask(Task task) {
            Debug.LogFormat("Task {0} triggered", task.Name);
            if (PlayerManager.LocalPlayer.AssignedTasks.Any(x => x.ID == task.ID && !x.IsCompleted)) {
                Debug.LogFormat("Task {0} assigned", task.Name);
                Task playerTask = PlayerManager.LocalPlayer.AssignedTasks.First(x => x.ID == task.ID && !x.IsCompleted);
                GameObject taskUI  = InstanceManager.DisplayFullscreen("Task UI Framework");
                TaskWindow taskPrefab = taskUI.GetComponent<TaskWindow>();
                taskPrefab.SetTask(playerTask);
            } else {
                Debug.LogFormat("Task {0} not assigned", task.Name);
            }
        }

        public static void Load() { }

        public static void AddTaskUpdateListener(UnityAction action) {
            onTaskUpdate.AddListener(action);
        }

        public static void RemoveTaskUpdateListener(UnityAction action) {
            onTaskUpdate.RemoveListener(action);
        }
        public static Task Task(string id) {
            if (Tasks.Any(x => x.GetID().ToLower() == id.ToLower())) {
                return Tasks.First(x => x.GetID().ToLower() == id.ToLower());
            } else {
                Debug.LogWarningFormat("No Task Found Named: {0}", id);
                return null;
            }
           
        }

        public static void TaskUpdated() {
            onTaskUpdate?.Invoke();
            PlayerManager.PlayerUpdated();
            TeamManager.TeamUpdated();
        }

    }
}