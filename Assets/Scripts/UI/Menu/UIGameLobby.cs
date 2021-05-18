using Game.Managers;
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

    public bool Ready { get; private set; }


    private void OnEnable() {
        if (!Ready) {
            MenuContainer = transform.Find("Menu Container").gameObject;

            TeamCodeContainer = MenuContainer.transform.Find("Team Code Container").gameObject;

            LobbyContainer = MenuContainer.transform.Find("Lobby Container").gameObject;

            TeamManager.AddTeamUpdateListener(UpdateTeams);
            Ready = true;
        }
    }

    // Start is called before the first frame update
    void Start() {
        UpdateTeams();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnDestroy() {
        TeamManager.RemoveTeamUpdateListener(UpdateTeams);
    }

    private void UpdateTeams() {
        foreach(Team team in TeamManager.Teams) {
            GameObject instance = InstanceManager.Instantiate("Lobby Team Cell", TeamCodeContainer);
            UICell cell = instance.GetComponent<UICell>();
            cell.SetRightText(team.GenCode);
            cell.SetLeftText(team.Name);
        }
    }
}
