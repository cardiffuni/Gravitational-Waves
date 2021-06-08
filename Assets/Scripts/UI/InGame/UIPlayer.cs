using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : MonoBehaviour
{
    private void OnEnable() {
        InstanceManager.Canvas = FindObjectOfType<Canvas>();
        InstanceManager.FullScreen = transform.Find("Fullscreen").gameObject;
        InstanceManager.WindowsSection = transform.Find("View Area").Find("Middle Section").Find("Windows Section").gameObject;
    }
    // Start is called before the first frame update
    void Start() {
        InstanceManager.Canvas = FindObjectOfType<Canvas>();
        InstanceManager.FullScreen = transform.Find("Fullscreen").gameObject;
        InstanceManager.WindowsSection = transform.Find("View Area").Find("Middle Section").Find("Windows Section").gameObject;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
