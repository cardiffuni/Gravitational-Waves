using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroOnClick : MonoBehaviour
{
    
    public void OnClick()
    {
        Destroy(gameObject);
    }
    
}
