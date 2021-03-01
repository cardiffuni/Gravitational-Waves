using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Network/NewNetworkDiscoveryHUD")]
[RequireComponent(typeof(NewNetworkDiscovery))]
public class NewNetworkDiscoveryHUD : MonoBehaviour
{
    readonly Dictionary<long, NetworkResponseMessage> discoveredServers = new Dictionary<long, NetworkResponseMessage>();
    Vector2 scrollViewPos = Vector2.zero;

    public NewNetworkDiscovery newNetworkDiscovery;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (newNetworkDiscovery == null)
        {
            newNetworkDiscovery = GetComponent<NewNetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(newNetworkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, newNetworkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    void OnGUI()
    {
        if (NetworkManager.singleton == null)
            return;

        if (NetworkServer.active || NetworkClient.active)
            return;

        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            DrawGUI();
    }

    void DrawGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Find Servers"))
        {
            discoveredServers.Clear();
            newNetworkDiscovery.StartDiscovery();
        }

        // LAN Host
        if (GUILayout.Button("Start Host"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            newNetworkDiscovery.AdvertiseServer();
        }

        // Dedicated server
        if (GUILayout.Button("Start Server"))
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();

            newNetworkDiscovery.AdvertiseServer();
        }

        GUILayout.EndHorizontal();

        // show list of found server

        GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

        // servers
        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

        foreach (NetworkResponseMessage info in discoveredServers.Values)
            if (GUILayout.Button(info.EndPoint.Address.ToString()))
                Connect(info);

        GUILayout.EndScrollView();
    }

    void Connect(NetworkResponseMessage info)
    {
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(NetworkResponseMessage info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }
}

