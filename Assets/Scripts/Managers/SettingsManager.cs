using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Managers {
    public static class SettingsManager {

        public static class TMPColour {
            public readonly static string White = "<color=#fff>";
            public readonly static string Black = "<color=#000>";
            public readonly static string Grey = "<color=#aaa>";
            public readonly static string Red = "<color=#f00>";
            public readonly static string Green = "<color=#0f0>";
            public readonly static string Blue = "<color=#00f>";
        }

        public static readonly Formatting JsonSerializerFormatting = Formatting.Indented;
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        public const string JsonDefaultPath = "Assets/Resources/Data/";

        public const float H1SizeMin = 26;
        public const float H1SizeMax = 36;
        public const float H2SizeMin = 18;
        public const float H2SizeMax = 28;
        public const float H3SizeMin = 10;
        public const float H3SizeMax = 20;

        public const float ParaSizeMin = 10;
        public const float ParaSizeMax = 12;
        public const float LogSizeMin = 10;
        public const float LogSizeMax = 16;
        public const float LabelSizeMin = 10;
        public const float LabelSizeMax = 14;
        public const float ValueSizeMin = 8;
        public const float ValueSizeMax = 12;

        public static readonly Color ButtonColorStandard = new Color32(233, 206, 129, 255);
        public const bool Midpoint = false;
        public const MidpointRounding MidpointRounding = (MidpointRounding)1;
        public const bool RoundDown = true;

        public const string taskListRowPrefab = "UITaskListRow";
        public const int taskDefaultNumber = 10;
        public const int taskMinNumber = 1;
        public const int taskMaxNumber = 20;

        public const int teamsDefaultNumber = 3;
        public const int teamsMinNumber = 1;
        public const int teamsMaxNumber = 10;

        public const int timeMDefaultNumber = 20;
        public const int timeMMinNumber = 5;
        public const int timeMMaxNumber = 1440;

        public const bool teamChatDefault = true;

        public const int GenCodeLen = 6;

        public static bool Ready { get; private set; }
        

        static SettingsManager() {
            Debug.Log("Loading SettingsManager");
            Ready = true;
        }

        public static void Load() { }

    }
}