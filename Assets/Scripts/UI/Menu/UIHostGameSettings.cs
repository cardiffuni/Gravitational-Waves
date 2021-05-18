using Game.Managers;
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

            Debug.Log("Setting hosting defaults");
            TeamManager.SetDefaults();
            TeamsCountField.text = SettingsManager.teamsDefaultNumber.ToString();
            TeamChatToggle.isOn = SettingsManager.teamChatDefault;
            TimeLimitField.text = SettingsManager.timeMDefaultNumber.ToString();

            TeamsCountField.onEndEdit.AddListener(UpdateTeamsCount);
            TeamChatToggle.onValueChanged.AddListener(UpdateTeamChat);
            TimeLimitField.onEndEdit.AddListener(UpdateTimeLimit);
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
}
