using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers.Controllers {
    public class Operator : MonoBehaviour { 
        public static bool Ready { get; private set; }

        // called zero
        void Awake() {
            Ready = true;
        }

        // called first
        void OnEnable() {
            //Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Debug.Log("OnSceneLoaded: " + scene.name);
            //Debug.Log(mode);
        }

        // called third, before the first frame update
        void Start() {
            //Debug.Log("Start");
        }

        // called forth, Update is called once per frame
        void Update() {

        }

        // called when the game is terminated
        void OnDisable() {
            Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}