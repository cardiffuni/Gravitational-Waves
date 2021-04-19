using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroOnClick : MonoBehaviour
{
    public GameObject Bottle;
    public GameObject Mirror;
    public void OnClick()
    {
        Destroy(Bottle);
        Mirror.GetComponent<Image>().color = new Color32(255, 170, 211, 0);
    }
    
}
