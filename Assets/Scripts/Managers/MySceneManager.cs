using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using static Game.Managers.UIManager;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Game.Managers {
    public static class MySceneManager {

        public static Scene Current { get => SceneManager.GetActiveScene(); }
        public static Stack<string> Previous { get; private set; }

        public static List<string> AdditonalScenes { get; private set; }

        private static List<string> OverMenuScenes;

        public static string overlay = "OverlayMenu";
        public static string mainMenu = "MainMenu";
        private static event UnityAction OnExit;

        public static UnityEvent onLoadOnce { get; private set; }


        public static bool Ready { get; private set; }


        static MySceneManager() {
            Debug.Log("Loading MySceneManager");
            AdditonalScenes = new List<string>();
            OverMenuScenes = new List<string>() { overlay, mainMenu };
            SceneManager.activeSceneChanged += OnSceneLoaded;
            Previous = new Stack<string>();
            onLoadOnce = new UnityEvent();
            Ready = true;
        }

        public static void Load() {
            
        }

        public static void LoadScene(string name) {
            OnExit?.Invoke();
            if (name != null && name != "") {
                Debug.LogFormat("Changing Scene from {0} to {1}", Current.name, name);
                Previous.Push(Current.name);
                AdditonalScenes.Clear();
                SceneManager.LoadScene(name, LoadSceneMode.Single);
                AdditonalScenes = new List<string>();
            } else {
                string error = name == null ? "Null String" : "Empty String";
                Debug.LogWarningFormat("{0} Scene Called", error);
            }

        }

        public static void OnSceneLoaded(Scene previousScene, Scene newScene) {
            InstanceManager.Init();
            onLoadOnce?.Invoke();
            onLoadOnce.RemoveAllListeners();
        }

        public static void LoadSceneAdditive(string name) {
            Debug.LogFormat("Loading Additive Scene {0}", name);
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
            AdditonalScenes.Add(name);
        }

        public static void UnloadScene(string name) {
            if (AdditonalScenes.Contains(name)) {
                Debug.LogFormat("Removing Scene {0}", name);
                AdditonalScenes.Remove(name);
                SceneManager.UnloadSceneAsync(name);
            }

        }

        public static void AddOnGameExitCallback(UnityAction callback) {
            OnExit += callback;
        }


        internal static void OverlayMenuToggle() {
            if (AdditonalScenes.Contains(overlay)) {
                UnloadOverlayMenu();
            } else {
                LoadOverlayMenu();
            }

        }
        internal static void LoadOverlayMenu() {
            if (!AdditonalScenes.Contains(overlay) && !OverMenuScenes.Contains(Current.name)) {
                LoadSceneAdditive(overlay);
            }
        }

        internal static void UnloadOverlayMenu() {
            if (AdditonalScenes.Contains(overlay)) {
                UnloadScene(overlay);
            }
        }


        public static void LoadPreviousScene() {
            Debug.Log(Previous.Peek());
            LoadScene(Previous.Pop());
            Previous.Pop();
        }

        public static void MenuExitGame(){
            OnExit?.Invoke();
            Debug.Log("Exiting Game");
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            Debug.Log("UnityEditor Stop");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("Application Quit");
            Application.Quit();
#endif
        }
        public static void AddLoadOnceListener(UnityAction action) {
            onLoadOnce.AddListener(action);
        }

        public static void RemoveLoadOnceListener(UnityAction action) {
            onLoadOnce.RemoveListener(action);
        }
    }
}