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

    public GameObject NavigationContainer { get; private set; }

    public Button StartBtn { get; private set; }

    public static bool Ready { get; private set; }

    private void OnEnable() {
        Debug.Log("UIConnectGame Enabled");
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            JoinCodeContainer = MenuContainer.transform.Find("Join Code Container").gameObject;
            JoinCodeField = JoinCodeContainer.GetComponentInChildren<TMP_InputField>();

            if(NetworkManager.ClientGenCode != null) {
                JoinCodeField.text = NetworkManager.ClientGenCode;
            }

            NavigationContainer = transform.Find("Navigation").gameObject;
            StartBtn = NavigationContainer.transform.Find("Start Button").GetComponent<Button>(); 



            JoinCodeField.onEndEdit.AddListener(UpdateJoinCode);
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

    private void UpdateJoinCode(string input) {
        string inputUpp = input.ToUpper();
        string inputFormatted = Regex.Replace(inputUpp, "[^A-Z]", String.Empty);
        string code = inputFormatted.Substring(0, Mathf.Min(inputFormatted.Length,SettingsManager.GenCodeLen));
        Debug.LogFormat("JoinCodeField {0}", code);
        JoinCodeField.text = code;
        NetworkManager.SetClientGenCode(code);
        if (code.Length == SettingsManager.GenCodeLen) {
            StartBtn.interactable = true;
        } else {
            StartBtn.interactable = false;
            Debug.LogWarningFormat("Code {0} is too short", code);
        }
    }
}
