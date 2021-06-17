using UnityEngine;
using System.Collections;
using TMPro;
using static Game.Managers.SettingsManager;
using Game.Managers;


[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(RectTransform))]
public class HoverText : MonoBehaviour {

    internal TextMeshPro Text { get; private set; }
    internal RectTransform Rect { get; private set; }
    internal string Content { get; private set; }

    private void OnEnable() {
        Text = gameObject.GetComponent<TextMeshPro>();
        Rect = gameObject.GetComponent<RectTransform>();
        //Text.fontSize = 6;
        Text.alignment = TextAlignmentOptions.Center;
        Rect.sizeDelta = new Vector2(5, 0.5f);
        Rect.localPosition += Vector3.up * 2f;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        UpdateRotation();
    }

    public void Set(string content) {
        Content = content;
        UpdateText();
    }

    private void UpdateRotation() {
        //TextMesh.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        //Vector3 movement = transform.position - new Vector3(CameraManager.Current.transform.position.x, 0, CameraManager.Current.transform.position.z);
        //if (movement != Vector3.zero) {
        //transform.rotation = Quaternion.LookRotation(movement);
        //}
        
        if (Camera.current != null) {
            transform.rotation = Quaternion.Euler(0, Camera.current.transform.rotation.eulerAngles.y, 0);
        }
    }

    private void UpdateText() {
        Text.text = string.Format("{0}{1}", TMPColour.White, Content);
    }
}