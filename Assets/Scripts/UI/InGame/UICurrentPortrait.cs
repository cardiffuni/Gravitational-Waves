using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using static Game.UI.UITemplates;
using static Game.UI.UIFunctions;
using Game.UI.InGame.Sections.Items;
using Game.Players;
using Game.Managers;

namespace Game.UI.InGame.Sections {
    public class UICurrentPortrait : MonoBehaviour {
        public GameObject PortraitPanel { get; private set; }
        public Image Portrait { get; private set; }
        public GameObject NamePanel { get; private set; }
        public TextMeshProUGUI Name { get; private set; }
        public GameObject DetailsPanel { get; private set; }
        public TextMeshProUGUI Details { get; private set; }
        public bool FirstLoadDone { get; protected set; }
        public bool Ready { get; protected set; }

        // Use this for initialization
        void Start() {
            if (!Ready) {
                Portrait = gameObject.transform.Find("Portrait").Find("Image").GetComponentInChildren<Image>();
                Portrait.color = new Color(1, 1, 1);


                Name = gameObject.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();

                Details = gameObject.transform.Find("Details").GetComponentInChildren<TextMeshProUGUI>();
                UpdateCurrentPlayer();
                PlayerManager.AddPlayerUpdateListener(UpdateCurrentPlayer);

                //UpdateCurrentCharacter();
                Ready = true;
            } else {
                Debug.LogWarning("UITurnDisplay Started More than once");
            }

        }


        // Update is called once per frame
        void Update() {
            if (!FirstLoadDone) {
                UpdateCurrentPlayer();
            }
        }

        public void SetPortrait(string spriteID) {
            Portrait.enabled = true;
            if (spriteID != null) {
                Portrait.sprite = AssetManager.Sprite(spriteID);
            }

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

        public void UpdateCurrentPlayer() {
            if (PlayerManager.LocalPlayer != null) {
                Player player = PlayerManager.LocalPlayer;
                SetPortrait(player.Thumbnail);
                SetName(player.Name);
                SetDesc(string.Format(player.GetDescription()));
                FirstLoadDone = true;
            } 
        }
    }
}