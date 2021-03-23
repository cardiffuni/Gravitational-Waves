using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers {
    public static class CameraManager {
        public static Camera Main => Camera.main;
        public static Camera Current { get; private set; }
        public static bool Ready { get; private set; }

        static CameraManager() {
            Debug.Log("Loading CameraManager");
            Reset();
            Ready = true;
        }

        public static void Load() { }


        public static void Overview() {
            Select(Main);
        }

        //public static void Select(Character character) {
            //Select(character.Instance.FirstPersonCamera);
        //}

        public static void Switch() {
            if (Current == Main) {
                //Select(TurnManager.Turn.Character);
            } else {
                Overview();
            }
        }

        public static void Select(Camera selection) {
            if (Current != selection) {
                //Current.gameObject.SetActive(false);
                Current.enabled = false;
                Current = selection;
                Current.enabled = true;
                //Current.gameObject.SetActive(true);
            }
        }

        public static void Reset() {
            Current = Main;
        }


    }
}