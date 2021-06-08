using Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHostGameSettings : MonoBehaviour {

    public GameObject MenuContainer { get; private set; }

    public GameObject TeamsCountContainer { get; private set; }
    public TMP_InputField TeamsCountField { get; private set; }

    public GameObject TeamChatContainer { get; private set; }
    public Toggle TeamChatToggle { get; private set; }

    public GameObject TimeLimitContainer { get; private set; }
    public TMP_InputField TimeLimitField { get; private set; }

    public GameObject InfoMessageContainer { get; private set; }
    public TextMeshProUGUI InfoMessageText { get; private set; }

    public GameObject NavigationContainer { get; private set; }

    public Button ConfirmBtn { get; private set; }
    public Button BackBtn { get; private set; }

    public bool Ready { get; private set; }

    private void OnEnable() {
        Debug.Log("UIHostGameSettings Enabled");
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            TeamsCountContainer = MenuContainer.transform.Find("Teams Count Container").gameObject;
            TeamsCountField = TeamsCountContainer.GetComponentInChildren<TMP_InputField>();
            

            TeamChatContainer = MenuContainer.transform.Find("Team Chat Container").gameObject;
            TeamChatToggle = TeamChatContainer.GetComponentInChildren<Toggle>();
            
            TimeLimitContainer = MenuContainer.transform.Find("Time Limit Container").gameObject;
            TimeLimitField = TimeLimitContainer.GetComponentInChildren<TMP_InputField>();

            InfoMessageContainer = MenuContainer.transform.Find("Info Message").gameObject;
            InfoMessageText = InfoMessageContainer.GetComponentInChildren<TextMeshProUGUI>();

            NavigationContainer = transform.Find("Navigation").gameObject;
            ConfirmBtn = NavigationContainer.transform.Find("Confirm Button").GetComponent<Button>();
            BackBtn = NavigationContainer.transform.Find("Back Button").GetComponent<Button>();

            Debug.Log("Setting hosting defaults");
            TeamManager.SetDefaults();
            TeamsCountField.text = SettingsManager.teamsDefaultNumber.ToString();
            TeamChatToggle.isOn = SettingsManager.teamChatDefault;
            TimeLimitField.text = SettingsManager.timeMDefaultNumber.ToString();

            TeamsCountField.onEndEdit.AddListener(UpdateTeamsCount);
            TeamChatToggle.onValueChanged.AddListener(UpdateTeamChat);
            TimeLimitField.onEndEdit.AddListener(UpdateTimeLimit);
            NetworkingManager.OnServerStarting.AddListener(ServerStarting);
            NetworkingManager.OnServerStarted.AddListener(ServerStarted);
            NetworkingManager.OnServerStopped.AddListener(ServerStopped);
            Ready = true;
        }
    }

    // Start is called before the first frame update
    void Start() {

        Debug.Log("UIHostGameSettings Started");

    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnDestroy() {
        TeamsCountField.onEndEdit.RemoveListener(UpdateTeamsCount);
        TeamChatToggle.onValueChanged.RemoveListener(UpdateTeamChat);
        TimeLimitField.onEndEdit.RemoveListener(UpdateTimeLimit);
        NetworkingManager.OnServerStarting.RemoveListener(ServerStarting);
        NetworkingManager.OnServerStarted.RemoveListener(ServerStarted);
        NetworkingManager.OnServerStopped.RemoveListener(ServerStopped);
    }

    private void UpdateTeamsCount(string input) {
        int num = int.Parse(input);

        num = Mathf.Max(num, SettingsManager.teamsMinNumber);
        num = Mathf.Min(num, SettingsManager.teamsMaxNumber);

        Debug.LogFormat("TeamsCountField {0}", num);
        TeamsCountField.text = num.ToString();
        TeamManager.SetTeamCount(num);
    }

    private void UpdateTeamChat(bool input) {
        Debug.LogFormat("TeamChatToggle {0}", input);
        TeamChatToggle.isOn = input;
        TeamManager.SetTeamChat(input);
    }

    private void UpdateTimeLimit(string input) {
        int num = int.Parse(input);

        num = Mathf.Max(num, SettingsManager.timeMMinNumber);
        num = Mathf.Min(num, SettingsManager.timeMMaxNumber);

        Debug.LogFormat("TimeLimitField {0}", num);
        TimeLimitField.text = num.ToString();
        TeamManager.SetTimeLimit(num);
    }

    private void ServerStarting() {
        string info = string.Format("Server Starting...");
        InfoMsg(info);
        DisableInteractables();
    }

    private void ServerStarted() {
        string info = string.Format("Server Started, getting data from server...");
        InfoMsg(info);
        NetworkingManager.GetTeamGenCodes(TeamsSet);
        NetworkingManager.NetworkingController.PlayerView = "LIGOMainBuilding";
        NetworkingManager.NetworkingController.autoCreatePlayer = true;
    }

    private void ServerStopped() {
        string info = string.Format("Server Stopped.");
        EnableInteractables();
        ErrorMsg(info);
    }

    private void TeamsSet(bool working, string message) {
        if (working) {
            GoodMsg(message);
            StartCoroutine(NetworkingManager.LoadLobby());
            InfoMsg("Server Starting...");
        } else {
            ErrorMsg(message);
            EnableInteractables();
        }
    }

    private void ErrorMsg(string msg) {
        Debug.LogFormat(msg);
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3921569f);
        InfoMessageText.text = msg;
    }

    private void WarnMsg(string msg) {
        Debug.LogFormat(msg);
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 1f, 0f, 0.3921569f);
        InfoMessageText.text = msg;
    }
    private void InfoMsg(string msg) {
        Debug.LogFormat(msg);
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3921569f);
        InfoMessageText.text = msg;
    }

    private void GoodMsg(string msg) {
        Debug.LogFormat(msg);
        InfoMessageContainer.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3921569f);
        InfoMessageText.text = msg;
    }

    private void EnableInteractables() {
        ConfirmBtn.interactable = true;
        BackBtn.interactable = true;
        TeamChatToggle.interactable = true;
        TeamsCountField.interactable = true;
        TimeLimitField.interactable = true;
    }

    private void DisableInteractables() {
        ConfirmBtn.interactable = false;
        BackBtn.interactable = false;
        TeamChatToggle.interactable = false;
        TeamsCountField.interactable = false;
        TimeLimitField.interactable = false;
    }
}
