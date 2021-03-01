using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ServerListManager")]
[RequireComponent(typeof(NewNetworkDiscovery))]
public class ServerListManager : MonoBehaviour
{
    public NewNetworkDiscovery newNetworkDiscovery;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (newNetworkDiscovery == null)
        {
            newNetworkDiscovery = GetComponent<NewNetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(newNetworkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, newNetworkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    private void OnDiscoveredServer(NetworkResponseMessage info)
    {
        AddServer(info);
    }

    public static readonly Dictionary<string, NetworkResponseMessage> DiscoveredServers = new Dictionary<string, NetworkResponseMessage>();

    public static void AddServer(NetworkResponseMessage serverData)
    {
        DiscoveredServers.Add(serverData.gameCode, serverData);
    }

    public static void RemoveServerByCode(string gameCode)
    {
        DiscoveredServers.Remove(gameCode);
    }
}
