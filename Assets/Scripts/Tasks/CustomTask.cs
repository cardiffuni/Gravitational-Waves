using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Game.Utility;
using UnityEngine.Networking;
using Game.Managers;

namespace Game.Tasks {
    [Serializable]
    public class CustomTask : Task {
        public string Question { get; protected set; }
        public string ImageURL { get; protected set; }
        [JsonIgnore]
        public Texture2D image { get; protected set; }
        public List<string> MultiChoiceAnswers { get; protected set; }
        public string CorrectAnswerString { get; protected set; }
        public int CorrectAnswerMultiChoice { get; protected set; }
        public float CorrectAnswerNumber { get; protected set; }
        public float CorrectAnswerNumberVariance { get; protected set; }

        [JsonConstructor]
        public CustomTask(string Question, string ImageURL, List<string> MultiChoiceAnswers, string CorrectAnswerString, int CorrectAnswerMultiChoice, float CorrectAnswerNumber, float CorrectAnswerNumberVariance, bool IsInProgress, bool IsCompleted, string ID, string Name, string Description, string Prefab, string Reward) : base(IsInProgress, IsCompleted, ID, Name, Description, Prefab, Reward) {
            this.Question = Question;
            this.ImageURL = ImageURL;
            this.MultiChoiceAnswers = MultiChoiceAnswers;
            this.CorrectAnswerString = CorrectAnswerString;
            this.CorrectAnswerMultiChoice = CorrectAnswerMultiChoice;
            this.CorrectAnswerNumber = CorrectAnswerNumber;
            this.CorrectAnswerNumberVariance = CorrectAnswerNumberVariance;

            GetImageFromURL(ImageURL);
        }

        public CustomTask(string question, string imageURL, List<string> multiChoiceAnswers, string correctAnswerString, int correctAnswerMultiChoice, float correctAnswerNumber, float correctAnswerNumberVariance, string id, string name, string description, string prefab, string reward) : base(id, name, description, prefab, reward) {
            Question = question;
            ImageURL = imageURL;
            MultiChoiceAnswers = multiChoiceAnswers;
            CorrectAnswerString = correctAnswerString;
            CorrectAnswerMultiChoice = correctAnswerMultiChoice;
            CorrectAnswerNumber = correctAnswerNumber;
            CorrectAnswerNumberVariance = correctAnswerNumberVariance;
        }

        public CustomTask(CustomTask task) : base(task) {
            Question = task.Question;
            ImageURL = task.ImageURL;
            MultiChoiceAnswers = task.MultiChoiceAnswers;
            CorrectAnswerString = task.CorrectAnswerString;
            CorrectAnswerMultiChoice = task.CorrectAnswerMultiChoice;
            CorrectAnswerNumber = task.CorrectAnswerNumber;
            CorrectAnswerNumberVariance = task.CorrectAnswerNumberVariance;

            GetImageFromURL(ImageURL);
        }

        public override Task Clone() {
            return new CustomTask(this);
        }

        private void GetImageFromURL(string url) {
            InstanceManager.InstanceController.StartCoroutine(Networking.GetRequest(url, SetImageFromGet));

        }

        private void SetImageFromGet(UnityWebRequest request) {
            if (request != null) {
                Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (texture != null) {
                    image = (Texture2D)texture;
                }
            }
            if (image == null) {
                Debug.LogWarningFormat("Texture for URL {0}, not available", ImageURL);
            }
        }
    }
}