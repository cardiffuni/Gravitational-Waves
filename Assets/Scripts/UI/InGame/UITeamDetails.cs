using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using static Game.UI.UITemplates;
using static Game.UI.UIFunctions;
using Game.UI.InGame.Sections.Items;
using Game.Players;
using Game.Managers;
using Game.Teams;

namespace Game.UI.InGame.Sections {
    public class UITeamDetails : MonoBehaviour {
        public GameObject NamePanel { get; private set; }
        public TextMeshProUGUI Name { get; private set; }
        public GameObject DetailsPanel { get; private set; }
        public TextMeshProUGUI Details { get; private set; }

        public bool FirstLoadDone { get; protected set; }
        public bool Ready { get; protected set; }

        // Use this for initialization
        void Start() {
            if (!Ready) {

                Name = gameObject.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();

                Details = gameObject.transform.Find("Details").GetComponentInChildren<TextMeshProUGUI>();
                UpdateTeamDetails();
                TeamManager.AddTeamUpdateListener(UpdateTeamDetails);

                //UpdateCurrentCharacter();
                Ready = true;
            }

        }


        // Update is called once per frame
        void Update() {
            if (!FirstLoadDone) {
                UpdateTeamDetails();
            }
        }

        private void OnDestroy() {
            TeamManager.RemoveTeamUpdateListener(UpdateTeamDetails);
        }

        public void SetName(string text) {
            gameObject.name = text;
            Name.enabled = true;
            Name.text = string.Format("{0}{1}", SettingsManager.TMPColour.Black, text);
        }

        public void SetDesc(string text) {
            Details.enabled = true;
            Details.text = string.Format("{0}{1}", SettingsManager.TMPColour.Black, text);
        }

        public void UpdateTeamDetails() {
            
            if (PlayerManager.LocalPlayer != null && PlayerManager.LocalPlayer.Team != null) {
                Team team = PlayerManager.LocalPlayer.Team;
                SetName(team.Name);
                SetDesc(team.GetDescription());
                FirstLoadDone = true;
            } 
        }
    }
}