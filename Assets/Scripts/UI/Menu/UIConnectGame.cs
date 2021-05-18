using Game.Managers;
using kcp2k;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectGame : MonoBehaviour {

    public GameObject MenuContainer { get; private set; }

    public GameObject JoinCodeContainer { get; private set; }
    public TMP_InputField JoinCodeField { get; private set; }

    public GameObject InfoMessageContainer { get; private set; }
    public TextMeshProUGUI InfoMessageText { get; private set; }

    public GameObject NavigationContainer { get; private set; }

    public Button StartBtn { get; private set; }

    public bool Ready { get; private set; }

    private void OnEnable() {
        Debug.Log("UIConnectGame Enabled");
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            JoinCodeContainer = MenuContainer.transform.Find("Join Code Container").gameObject;
            JoinCodeField = JoinCodeContainer.GetComponentInChildren<TMP_InputField>();

            if (NetworkManager.ClientGenCode != null) {
                JoinCodeField.text = NetworkManager.ClientGenCode;
            }

            InfoMessageContainer = MenuContainer.transform.Find("Info Message").gameObject;
            InfoMessageText = InfoMessageContainer.GetComponentInChildren<TextMeshProUGUI>();

            NavigationContainer = transform.Find("Navigation").gameObject;
            StartBtn = NavigationContainer.transform.Find("Start Button").GetComponent<Button>();

            

            JoinCodeField.onEndEdit.AddListener(UpdateJoinCode);
            NetworkManager.OnClientConnecting.AddListener(Connecting);
            NetworkManager.OnClientDisconnect.AddListener(CantConnect);
            NetworkManager.OnClientError.AddListener(ErrorConnect);
            Ready = true;
        }
    }

    // Start is called before the first frame update
    void Start() {

        Debug.Log("UIConnectGame Started");

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnDestroy() {
        NetworkManager.OnClientConnecting.RemoveListener(Connecting);
        NetworkManager.OnClientDisconnect.RemoveListener(CantConnect);
        NetworkManager.OnClientError.RemoveListener(ErrorConnect);
    }
    

    private void UpdateJoinCode(string input) {
        string inputUpp = input.ToUpper();
        string inputFormatted = Regex.Replace(inputUpp, "[^A-Z]", String.Empty);
        string code = inputFormatted.Substring(0, Mathf.Min(inputFormatted.Length, SettingsManager.GenCodeLen));
        Debug.LogFormat("JoinCodeField {0}", code);
        JoinCodeField.text = code;

        if (code.Length == SettingsManager.GenCodeLen) {
            NetworkManager.SetClientGenCode(code);
            NetworkManager.CheckAndGetIP(code, VerifyJoinCode);
            
        } else {
            StartBtn.interactable = false;
            Debug.LogWarningFormat("Code {0} is too short", code);
            CodeFormat();
        }
    }
    private void VerifyJoinCode(bool success, string input) {
        if (success) {
            if (input != null && input.Length >= 7) {
                NetworkManager.SetClientHostIP(input);
                ValidCode();
                StartBtn.interactable = true;
            } else {
                StartBtn.interactable = false;
                Debug.LogFormat("IP '{0}' invalid", input);
                InvalidCode();
            }
        } else {
            StartBtn.interactable = false;
            Debug.LogFormat(input);
            InvalidCode();
        }

    }

    private void CodeFormat() {
        string info = string.Format("The code needs to be 6 Letters A-Z");
        Debug.LogFormat(info);
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 1f, 0f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = info;
    }

    private void ValidCode() {
        string info = string.Format("Valid Code");
        Debug.LogFormat(info);
        InfoMessageContainer.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = info;
    }

    private void InvalidCode() {
        string info = string.Format("Invalid Code");
        Debug.LogFormat(info);
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = info;
    }

    private void Connecting() {
        string info = string.Format("Connecting to Host...");
        Debug.LogFormat(info);
        StartBtn.interactable = false;
        JoinCodeField.interactable = false;
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = info;
    }

    private void CantConnect() {
        string error = string.Format("Cannot Connect to Host");
        Debug.LogFormat(error);
        StartBtn.interactable = true;
        JoinCodeField.interactable = true;
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = error;
    }

    private void ErrorConnect() {
        string error = string.Format("Error Connecting to Host");
        Debug.LogFormat(error);
        StartBtn.interactable = true;
        JoinCodeField.interactable = true;
        InfoMessageContainer.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3921569f);
        InfoMessageContainer.SetActive(true);
        InfoMessageText.text = error;
    }
}
