using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tasks {
    public class TaskWindow : MonoBehaviour {


        public GameObject Window { get; private set; }
        public Button CloseWindowBtn { get; private set; }
        public GameObject TaskContainer { get; private set; }
        public ITaskPrefab TaskInstance { get; private set; }
        public TMP_Text TaskTitle { get; private set; }

        public Task Task { get; private set; }

        public bool Ready { get; private set; }

        private void OnEnable() {
            if (!Ready) {

                CloseWindowBtn = transform.Find("Close Button").GetComponent<Button>();

                Window = transform.Find("Window").gameObject;
                TaskTitle = Window.transform.Find("Title").GetComponentInChildren<TMP_Text>();
                TaskContainer = Window.transform.Find("Container").gameObject;

                CloseWindowBtn.onClick.AddListener(CloseWindow);
                Ready = true;
            }
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnDestroy() {
            CloseWindowBtn.onClick.RemoveAllListeners();
        }

        public void SetTask(Task task) {
            Task = task;
            TaskTitle.text = task.Name;
            TaskInstance = InstanceManager.Instantiate(task.Prefab, TaskContainer).GetComponent<ITaskPrefab>();
            TaskInstance.SetParent(this);
        }

        public void CompleteTask() {
            Debug.LogFormat("Task {0} Completed, closing window", Task.Name);
            Task.Complete();
            CloseWindow();
        }

        public void CloseWindow() {
            Destroy(gameObject);
        }
    }
}