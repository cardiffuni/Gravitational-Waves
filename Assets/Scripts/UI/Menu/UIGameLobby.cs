using Game.Managers;
using Game.Players;
using Game.Teams;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLobby : MonoBehaviour {
    public GameObject MenuContainer { get; private set; }

    public GameObject TeamCodeContainer { get; private set; }

    public GameObject LobbyContainer { get; private set; }

    public ScrollRect LobbyScrollRect { get; private set; }
    public GameObject LobbyContent { get; private set; }

    public GameObject NavigationContainer { get; private set; }

    public Button StartBtn { get; private set; }
    public Button BackBtn { get; private set; }

    private int interval = 30;

    public bool Ready { get; private set; }


    private void OnEnable() {
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            TeamCodeContainer = MenuContainer.transform.Find("Team Code Container").gameObject;

            LobbyContainer = MenuContainer.transform.Find("Lobby Container").gameObject;

            LobbyScrollRect = LobbyContainer.transform.GetComponentInChildren<ScrollRect>();

            LobbyContent = LobbyScrollRect.content.gameObject;

            NavigationContainer = transform.Find("Navigation").gameObject;
            StartBtn = NavigationContainer.transform.Find("Start Button").GetComponent<Button>();
            BackBtn = NavigationContainer.transform.Find("Back Button").GetComponent<Button>();

            TeamManager.AddTeamUpdateListener(UpdateTeams);
            PlayerManager.AddPlayerUpdateListener(UpdateLobby);

            Ready = true;
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (NetworkingManager.isServer) {
            Debug.Log("is server");
            TeamCodeContainer.SetActive(true);
            StartBtn.gameObject.SetActive(true);
            UpdateTeams();
        } else {
            Debug.Log("is not server");
            TeamCodeContainer.SetActive(false);
        }
    }

    private float nextActionTime = 0.0f;
    private float period = 10.0f;

    // Update is called once per frame
    void Update() {
        if (Time.frameCount % interval == 0) {
            UpdateLobby();
        }
    }

    private void OnDestroy() {
        TeamManager.RemoveTeamUpdateListener(UpdateTeams);
        PlayerManager.RemovePlayerUpdateListener(UpdateLobby);
    }

    private void UpdateTeams() {
        foreach (Transform child in TeamCodeContainer.transform) {
            Destroy(child.gameObject);
        }
        foreach (Team team in TeamManager.Teams) {
            if (NetworkingManager.isServer) {
                GameObject instance = InstanceManager.Instantiate("Lobby Team Cell", TeamCodeContainer);
                UICell cell = instance.GetComponent<UICell>();
                cell.SetRightText(team.GenCode);
                cell.SetLeftText(team.Name);
            }
        }
    }

    private void UpdateLobby() {
        foreach (Transform child in LobbyContent.transform) {
            Destroy(child.gameObject);
        }
        if (PlayerManager.Players != null) {
            foreach (Player player in PlayerManager.Players) {
                GameObject instance = InstanceManager.Instantiate("Lobby Player Row", LobbyContent);
                UIRow row = instance.GetComponent<UIRow>();
                row.SetRightText(player.Team.Name);
                row.SetLeftText(player.Name);
            }
        }

    }
}
