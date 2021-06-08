using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using Game.Extensions;
using Newtonsoft.Json;
using Game.Utility;
using Game.Tasks;

namespace Game.Managers {
    public static class AssetManager {

        public static List<Texture2D> Textures { get; private set; }
        public static List<Sprite> Sprites { get; private set; }
        public static List<TMP_FontAsset> Fonts { get; private set; }
        public static List<TextAsset> Texts { get; private set; }
        public static List<TextAsset> JSONs { get; private set; }
        public static List<GameObject> Prefabs { get; private set; }
        public static List<Task> Tasks { get; private set; }
        public static List<WaveData> Waves { get; private set; }
        public static bool Ready { get; private set; }
        static AssetManager() {
            Debug.Log("Loading AssetManager");
            Textures = new List<Texture2D>();
            Sprites = new List<Sprite>();
            Fonts = new List<TMP_FontAsset>();
            Texts = new List<TextAsset>();
            JSONs = new List<TextAsset>();
            Prefabs = new List<GameObject>();
            Tasks = new List<Task>();
            Waves = new List<WaveData>();

            InitTextures();
            InitAllSprites();
            InitFontAssets();
            InitTextAssets();
            InitJSONAssets();
            InitPrefabAssets();
            NetworkingManager.GetFolderData<Task>(SettingsManager.tasksURL, InitTasks);
            NetworkingManager.GetFolderData<WaveData>(SettingsManager.wavesURL, InitWaves);

            Ready = true;

        }

        public static void Load() { }

        private static void InitTextures() {
            List<string> log = new List<string>();
            List<Texture2D> textures = new List<Texture2D>();
            textures.AddRange(Resources.LoadAll<Texture2D>("Textures").Cast<Texture2D>().ToList());
            textures.AddRange(Resources.LoadAll<Texture2D>("Images").Cast<Texture2D>().ToList());
            foreach (Texture2D texture in textures) {
                Textures.Add(texture);
                string name = texture.name != "" ? string.Format("{0}-tex", texture.name) : string.Format("Sprite-{0}", Sprites.Count);
                log.Add(texture.name);
                Sprite sprite = UnityEngine.Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                sprite.name = name;
                Sprites.Add(sprite);
            }
            Debug.LogFormat("Textures: {0}", log.Commaise());

        }

        private static void InitAllSprites() {
            List<string> log = new List<string>();
            //foreach (Sprite asset in Resources.FindObjectsOfTypeAll<Sprite>()) {
            //    log.Add(asset.name);
            //    Sprites.Add(asset);
            //}
            List<Sprite> sprites = new List<Sprite>();
            sprites.AddRange(Resources.LoadAll<Sprite>("Sprites").Cast<Sprite>().ToList());
            foreach (Sprite asset in sprites) {
                log.Add(asset.name);
                Sprites.Add(asset);
            }
            Debug.LogFormat("Sprites: {0}", log.Commaise());
        }

        private static void InitFontAssets() {
            List<string> log = new List<string>();
            //foreach (TMP_FontAsset asset in Resources.FindObjectsOfTypeAll<TMP_FontAsset>()) {
            //    log.Add(asset.name);
            //    Fonts.Add(asset);
            //}
            List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();
            fonts.AddRange(Resources.LoadAll<TMP_FontAsset>("Fonts").Cast<TMP_FontAsset>().ToList());
            foreach (TMP_FontAsset asset in fonts) {
                log.Add(asset.name);
                Fonts.Add(asset);
            }
            Debug.LogFormat("Fonts: {0}", log.Commaise());
        }

        private static void InitTextAssets() {
            List<string> log = new List<string>();
            List<TextAsset> textFiles = new List<TextAsset>();
            textFiles.AddRange(Resources.LoadAll<TextAsset>("Text").Cast<TextAsset>().ToList());
            foreach (TextAsset asset in textFiles) {
                log.Add(asset.name);
                Texts.Add(asset);
            }
            Debug.LogFormat("Texts: {0}", log.Commaise());
        }

        private static void InitJSONAssets() {
            List<string> log = new List<string>();
            List<TextAsset> jsonFiles = new List<TextAsset>();
            jsonFiles.AddRange(Resources.LoadAll<TextAsset>("Data").Cast<TextAsset>().ToList());
            foreach (TextAsset asset in jsonFiles) {
                log.Add(asset.name);
                JSONs.Add(asset);
            }
            Debug.LogFormat("JSONs: {0}", log.Commaise());
        }

        private static void InitPrefabAssets() {
            List<string> log = new List<string>();
            List<GameObject> prefabs = new List<GameObject>();
            prefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs").Cast<GameObject>().ToList());
            foreach (GameObject asset in prefabs) {
                log.Add(asset.name);
                Prefabs.Add(asset);
            }
            Debug.LogFormat("Prefabs: {0}", log.Commaise());
        }
        private static void InitTasks(Dictionary<string, Task> data) {
            List<string> log = new List<string>();
            foreach (KeyValuePair<string, Task> item in data) {
                Task task = item.Value;
                log.Add(task.Name);
                Tasks.Add(task);
            }
            Debug.LogFormat("Tasks: {0}", log.Commaise());
        }
        private static void InitWaves(Dictionary<string, WaveData> data) {
            List<string> log = new List<string>();
            foreach (KeyValuePair<string, WaveData> item in data) {
                WaveData wave = item.Value;
                wave.SetName(item.Key);
                wave.SetID(item.Key);
                log.Add(wave.Name);
                Waves.Add(wave);
            }
            Debug.LogFormat("Waves: {0}", log.Commaise());
        }

        public static Texture2D Texture(string name) {
            if (!Textures.Any(x => x.name.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No Texture Found Named: {0}", name);
            }
            return Textures.DefaultIfEmpty(Textures[0]).First(x => x.name.ToLower() == name.ToLower());
        }

        public static Sprite Sprite(string name) {
            if (Sprites.Any(x => x == null)) {
                Debug.LogWarning("Null found in sprites, spring cleaning...");
                Sprites = Sprites.Where(x => x != null).ToList();
            }

            if (!Sprites.Any(x => x.name?.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No Sprite Found Named: {0}", name);
                return Sprites[0];
            } else {
                return Sprites.First(x => x.name.ToLower() == name.ToLower());
            }
            

        }
        public static TMP_FontAsset Font(string name) {
            if (!Fonts.Any(x => x.name.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No Font Found Named: {0}", name);
            }
            return Fonts.DefaultIfEmpty(Fonts[0]).FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }
        public static TextAsset Text(string name) {
            if (!Texts.Any(x => x.name.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No Text Named: {0}", name);
            }
            return Texts.DefaultIfEmpty(Texts[0]).FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }
        
        public static T JSON<T>(string name) {
            if (!JSONs.Any(x => x.name.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No JSON Named: {0}", name);
                return default;
            } else {
                return Functions.ReadJson<T>(JSONs.First(x => x.name.ToLower() == name.ToLower()));
            }
        }

        public static GameObject Prefab(string name) {
            if (!Prefabs.Any(x => x.name.ToLower() == name.ToLower())) {
                Debug.LogWarningFormat("No Prefab Named: {0}", name);
            }
            return Prefabs.DefaultIfEmpty(Prefabs[0]).FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }

        public static Task Task(string id) {
            if (!Tasks.Any(x => x.ID.ToLower() == id.ToLower())) {
                Debug.LogWarningFormat("No Task with ID: {0}", id);
            }
            return Tasks.DefaultIfEmpty(Tasks[0]).FirstOrDefault(x => x.ID.ToLower() == id.ToLower());
        }

        public static WaveData Wave(string id) {
            if (!Waves.Any(x => x.ID.ToLower() == id.ToLower())) {
                Debug.LogWarningFormat("No Wave with ID: {0}", id);
            }
            return Waves.DefaultIfEmpty(Waves[0]).FirstOrDefault(x => x.ID.ToLower() == id.ToLower());
        }

        public static string Assets() {
            return Sprites.Select(x => x.name).Commaise();
        }
    }
}