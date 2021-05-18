using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;

public class testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void runME() {
        Debug.LogFormat("testing {0}", "runME");
        //StartCoroutine (NetworkManager.GetCodes(3));
        //StartCoroutine(NetworkManager.GetIP("PQSTYD"));
    }
}
