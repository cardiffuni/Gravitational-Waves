using Game.Managers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHostGameMenu : MonoBehaviour
{
    public void HostGame()
    {
        var newNetworkDiscovery = GameObject.FindObjectOfType<NewNetworkDiscovery>();
        MySceneManager.LoadScene("GameView");
        NetworkManager.singleton.StartHost();
        newNetworkDiscovery.AdvertiseServer();

    }
}
