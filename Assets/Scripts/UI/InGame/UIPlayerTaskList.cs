using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Game.Managers;
using Game.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;

public class UIPlayerTaskList : MonoBehaviour {
    private GameObject taskListRowPrefab;

    private ScrollRect taskListScrollView;
    private GameObject taskListContainer;
    

    private List<UITaskRow> displayedTasks;

    public bool FirstLoadDone { get; private set; }
    public bool Ready { get; private set; }

    private void OnEnable() {

        if (!Ready) {
            taskListRowPrefab = AssetManager.Prefab(SettingsManager.taskListRowPrefab);
            taskListScrollView = gameObject.GetComponentInChildren<ScrollRect>();
            taskListContainer = taskListScrollView.content.gameObject;
            displayedTasks = new List<UITaskRow>();
            UpdateTasks();
            TaskManager.AddTaskUpdateListener(UpdateTasks);
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    void Update() {
        if (!FirstLoadDone) {
            UpdateTasks();
        }
    }

    private void UpdateTasks() {
        if (PlayerManager.LocalPlayer != null) {
            foreach (Task task in PlayerManager.LocalPlayer.AssignedTasks) {
                if (!displayedTasks.Any(x => x.Task == task)) {
                    GameObject isntance = Instantiate(taskListRowPrefab, taskListContainer.transform);
                    Debug.Log(isntance.name);
                    UITaskRow taskRow = isntance.GetComponent<UITaskRow>();
                    taskRow.AssignTask(task);
                    displayedTasks.Add(taskRow);
                }
            }
            FirstLoadDone = true;
        }
    }
}
