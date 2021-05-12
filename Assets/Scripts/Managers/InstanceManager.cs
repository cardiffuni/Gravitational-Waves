using Game.Managers.Controllers;
using Game.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using Mirror;

namespace Game.Managers {
    public static class InstanceManager {

        public static GameObject Operator { get; private set; }

        public static InstanceController InstanceController { get; private set; }
        public static KeyController KeyController { get; private set; }
        public static MouseController MouseController { get; private set; }
        public static Mirror.NetworkManager NetworkController { get; private set; }

        public static GameObject TempStorage { get; private set; }
        public static GameObject CharactersContainer { get; private set; }
        public static GameObject ObjectInstancesContainer { get; private set; }
        public static Canvas Canvas { get; private set; }
        public static GameObject FullScreen { get; private set; }
        public static GameObject WindowsSection { get; private set; }

        public static List<GameObject> GameObjects { get; private set; }

        public static bool Ready { get; private set; }

        static InstanceManager() {
            Init();
            Debug.Log("Loading InstanceManager");
        }

        public static void Load() { }

        public static void Init(GameObject parent = null) {
            if (parent == null) {
                Operator = new GameObject("Operator", typeof(Operator));
            } else {
                Operator = parent;
            }

            InstanceController = CreateController<InstanceController>("Instance Controller");
            KeyController = CreateController<KeyController>("Key Controller");
            MouseController = CreateController<MouseController>("Mouse Controller");
            NetworkController = CreateController<Mirror.NetworkManager>("Network Controller");

            TempStorage = new GameObject("Temp Storage");
            TempStorage.transform.SetParent(Operator.transform);

            Canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            Debug.Log(Canvas);
            CameraManager.Reset();
            InitGameObjects();
            if (Canvas.transform.Find("Player UI")) {
                FullScreen = Canvas.transform.Find("Player UI").Find("Fullscreen").gameObject;
                WindowsSection = Canvas.transform.Find("Player UI").Find("View Area").Find("Middle Section").Find("Windows Section").gameObject;
            } else {
                FullScreen = null;
                WindowsSection = null;
            }

            Ready = true;
        }

        public static void SetupCharacters() {
            CharactersContainer = new GameObject("Characters");
            CharactersContainer.transform.SetParent(Operator.transform);
        }

        private static void InitGameObjects() {
            ObjectInstancesContainer = new GameObject("Assets");
            ObjectInstancesContainer.transform.SetParent(Operator.transform);

            GameObjects = new List<GameObject>();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
            cube.transform.SetParent(ObjectInstancesContainer.transform);
            cube.SetActive(false);
            GameObjects.Add(cube);

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Capsule";
            capsule.transform.SetParent(ObjectInstancesContainer.transform);
            Vector3 newScale = new Vector3(0.8f, 1.2f, 0.8f);
            capsule.transform.localScale = newScale;
            CapsuleCollider collider = capsule.GetComponent<CapsuleCollider>();
            collider.height = newScale.y * 2;
            collider.radius = 0.5f;
            capsule.SetActive(false);
            GameObjects.Add(capsule);
        }

        public static GameObject GetGameObject(string name) {
            return GameObjects.First(x => x.name == name);
        }

        public static GameObject InstantiateActor(string name, GameObject parent) {
            GameObject obj = UnityEngine.Object.Instantiate(GetGameObject(name), parent.transform);
            obj.name = "Body";
            obj.SetActive(true);
            return obj;
        }

        public static GameObject DisplayFullscreen(string name) {
            if (FullScreen) {
                return Instantiate(name, FullScreen);
            } else {
                return null;
            }
            
        }
        
        public static GameObject DisplayWindow(string name) {
            if (FullScreen) {
                return Instantiate(name, WindowsSection);
            } else {
                return null;
            }
        }

        internal static GameObject Instantiate(string prefabID, GameObject parent) {
            return Instantiate(prefabID, parent.transform);
        }

        internal static GameObject Instantiate(string prefabID, Transform parent) {
            GameObject prefab = AssetManager.Prefab(prefabID);
            GameObject instance = GameObject.Instantiate(prefab, parent);
            return instance;
        }
        internal static T CreateController<T>(string name) where T : Component {
            GameObject controllerObject = new GameObject(name);
            controllerObject.transform.SetParent(Operator.transform);
            T controller = controllerObject.AddComponent<T>();
            return controller;
        }
    }
}