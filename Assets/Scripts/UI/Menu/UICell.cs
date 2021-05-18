using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICell : MonoBehaviour {

    public GameObject Container { get; private set; }
    public GameObject PanelLeft { get; private set; }
    public TextMeshProUGUI LeftText { get; private set; }
    public GameObject PanelRight { get; private set; }
    public TextMeshProUGUI RightText { get; private set; }

    public bool Ready { get; private set; }

    private void OnEnable() {
        if (!Ready) {
            Container = transform.Find("Container").gameObject;

            PanelLeft = Container.transform.Find("Panel Left").gameObject;
            LeftText = PanelLeft.GetComponentInChildren<TextMeshProUGUI>();
            PanelRight = Container.transform.Find("Panel Right").gameObject;
            RightText = PanelRight.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        
    }

    public void SetLeftText(string text) {
        LeftText.text = text;
    }

    public void SetRightText(string text) {
        RightText.text = text;
    }
}
