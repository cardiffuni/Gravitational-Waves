using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstContactApply : MonoBehaviour
{
    Graphic mirror;
    [SerializeField] [Range(0f, 1f)] float lerpTime;

    [SerializeField] Color myColor;

    public void ApplyFirstContact()
    {
        mirror = GetComponent<Graphic>();

        mirror.color = Color.Lerp(mirror.color, myColor, lerpTime);
        mirror.color = myColor;
    }
}
