using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Game.Managers {
    public static class ActionManager {
        public static Dictionary<ActionsEnum, UnityAction> ActionsCallbacks { get; private set; }

        public enum ActionsEnum { //Don't change a actions flag, it is what unity uses in seralisation in the inspector dropdown
            LoadSceneChooseGame = 0,
            LoadSceneJoinGame = 1,
            LoadSceneHostGame = 2,
            LoadSceneOptions = 3,
            LoadSceneMainMenu = 4,
            LoadSceneGameView = 5,
            LoadSceneLobby = 6,

            LobbyHost = 20,
            LobbyConnect = 21,

            InteractionWithObject = 51,
            InteractionWithArea = 52,

            SystemExit = 100,
            SystemLoadPreviousScene = 101,

            UILoadOverlayMenu = 150,
            UIUnloadOverlayMenu = 151,
        };


        public static bool Ready { get; private set; }

        public static void Load() { }


        static ActionManager() {
            Debug.Log("Loading ActionManager");

            ActionsCallbacks = new Dictionary<ActionsEnum, UnityAction>() {
                {ActionsEnum.LoadSceneChooseGame, LoadSceneChooseGame },
                {ActionsEnum.LoadSceneJoinGame, LoadSceneJoinGame },
                {ActionsEnum.LoadSceneHostGame, LoadSceneHostGame },
                {ActionsEnum.LoadSceneOptions, LoadSceneOptionsMenu },
                {ActionsEnum.LoadSceneMainMenu, LoadSceneMenuMain },
                {ActionsEnum.LoadSceneGameView, LoadSceneGameView },
                {ActionsEnum.LoadSceneLobby, LoadSceneLobby },

                {ActionsEnum.LobbyHost, LobbyHost },
                {ActionsEnum.LobbyConnect, LobbyConnect },

                {ActionsEnum.InteractionWithObject, InteractionWithObject },
                {ActionsEnum.InteractionWithArea, InteractionWithArea },

                {ActionsEnum.SystemExit, SystemExit },
                {ActionsEnum.SystemLoadPreviousScene, SystemLoadPreviousScene },

                {ActionsEnum.UILoadOverlayMenu, UILoadOverlayMenu },
                {ActionsEnum.UIUnloadOverlayMenu, UIUnloadOverlayMenu },

            };
            Ready = true;
        }

        public static void ActionsCallback(ActionsEnum action, UnityEvent listener) {
            if (ActionsCallbacks.ContainsKey(action)) {
                listener.AddListener(ActionsCallbacks[action]);
            } else {
                Debug.LogWarningFormat("No Action found for: {0}", action);
            }
        }

        public static void LoadSceneChooseGame() {
            Debug.Log("Action: Choose Game");
            MySceneManager.LoadScene("ChooseGame");
        }

        public static void LoadSceneJoinGame() {
            Debug.Log("Action: Join Game");
            MySceneManager.LoadScene("JoinGame");
        }

        public static void LoadSceneHostGame() {
            Debug.Log("Action: Host Game");
            MySceneManager.LoadScene("HostGame");
        }
        public static void LoadSceneOptionsMenu() {
            Debug.Log("Action: Options Menu");
            MySceneManager.LoadScene("OptionsMenu");
        }

        public static void LoadSceneMenuMain(){
            Debug.Log("Action: Main Menu");
            MySceneManager.LoadScene(MySceneManager.mainMenu);
        }

        public static void LoadSceneGameView() {
            Debug.Log("Action: Game View");
            MySceneManager.LoadScene("GameView");
        }

        public static void LoadSceneLobby() {
            Debug.Log("Action: Load Lobby");
            MySceneManager.LoadScene("GameLobby");
        }

        public static void LobbyConnect() {
            Debug.Log("Action: Connect To Lobby");
            //MySceneManager.AddLoadOnceListener(NetworkManager.StartClient);
            NetworkManager.StartClient();
        }

        public static void LobbyHost() {
            Debug.Log("Action: Host Lobby");
            MySceneManager.AddLoadOnceListener(NetworkManager.StartHost);
            LoadSceneLobby();
        }

        public static void InteractionWithObject() {
            Debug.Log("Action: Interaction With Object");
        }

        public static void InteractionWithArea() {
            Debug.Log("Action: Interaction With Area");
        }

        public static void SystemExit() {
            Debug.Log("Action: Quit");
            MySceneManager.MenuExitGame();
        }

        public static void SystemLoadPreviousScene() {
            Debug.Log("Action: Load Previous Scene");
            MySceneManager.LoadPreviousScene();
        }

        public static void UILoadOverlayMenu() {
            Debug.Log("Action: Load Overlay Menu");
            MySceneManager.LoadOverlayMenu();
        }
        public static void UIUnloadOverlayMenu() {
            Debug.Log("Action: Unload Overlay Menu");
            MySceneManager.UnloadOverlayMenu();
        }

    }
}