using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Managers.Controllers {
    public class GameController : MonoBehaviour {

        internal static GameObject GameObject;

        [Tooltip("Trigger Zone Prefab")]
        public GameObject Zone;

        [Scene]
        [Tooltip("Add all sub-scenes to this list")]
        public string[] subScenes;

        [Scene]
        public string gameScene;

        public bool Ready { get; private set; }

        private void OnEnable() {
            if (!Ready) {
                if (NetworkingManager.isServer) {
                }
                if (NetworkingManager.isClient || NetworkingManager.isHost) {
                }
            }
        }

        // Start is called before the first frame update
        void Start() {
            GameObject = gameObject;
            Ready = true;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}