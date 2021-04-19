using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroOnClick : MonoBehaviour
{
    public GameObject bottle;
    public Graphic mirror;
    public Color colour;

    public void Start()
    {
        mirror.GetComponent<Graphic>();
        colour = Color.white;
        mirror.color = colour;
    }
    public void OnClick()
    {
        Destroy(bottle);
        colour = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 4));
        mirror.color = colour;
    }
    
}
