using Game.Managers;
using Game.Players;
using Game.Teams;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLobby : MonoBehaviour {
    public GameObject MenuContainer { get; private set; }

    public GameObject TeamCodeContainer { get; private set; }

    public GameObject LobbyContainer { get; private set; }

    public GameObject NavigationContainer { get; private set; }

    public Button StartBtn { get; private set; }
    public Button BackBtn { get; private set; }


    public bool Ready { get; private set; }


    private void OnEnable() {
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            TeamCodeContainer = MenuContainer.transform.Find("Team Code Container").gameObject;

            LobbyContainer = MenuContainer.transform.Find("Lobby Container").gameObject;

            NavigationContainer = transform.Find("Navigation").gameObject;
            StartBtn = NavigationContainer.transform.Find("Start Button").GetComponent<Button>();
            BackBtn = NavigationContainer.transform.Find("Back Button").GetComponent<Button>();

            TeamManager.AddTeamUpdateListener(UpdateTeams);
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
        if (NetworkingManager.isClient && NetworkingManager.NetworkDataController != null) {
            if (Time.time > nextActionTime) {
                nextActionTime += period;
                Debug.LogFormat("Players size: {0}", PlayerManager.Players.Count);
                foreach (Player player in PlayerManager.Players) {
                    Debug.LogFormat("Player: Name {0}, ConnId {1}, Score {2}, Score {3}", player.Name, player.ConnId, player.Score, player.Score);
                }
                Debug.LogFormat("Team2 size: {0}", TeamManager.Teams.Count);
                foreach (Team team in TeamManager.Teams) {
                    Debug.LogFormat("Team: Name {0}, Id {1}, Score {2}, GenCode {3}", team.Name, team.ID, team.Score, team.GenCode);
                }
            }

        }
    }

    private void OnDestroy() {
        TeamManager.RemoveTeamUpdateListener(UpdateTeams);
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
}
