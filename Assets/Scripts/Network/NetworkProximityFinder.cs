using System.Collections.Generic;
using UnityEngine;
using Mirror;


/// <summary>
/// 
/// <para></para>
/// </summary>
[AddComponentMenu("Network/NetworkProximityFinder")]
[RequireComponent(typeof(NetworkIdentity))]
public class NetworkProximityFinder : NetworkBehaviour {

    public NetworkConnection LocalClientConn { get => NetworkClient.connection; }
    public Transform LocalClientTransform { get => LocalClientConn.identity.transform; }


    /// <summary>
    /// Used to find the distance to a Network connection of a player to the attached object.
    /// <para>The distance from the connection to the attached object is returned.</para>
    /// </summary>
    /// <param name="conn">Network connection of a player.</param>
    public float DistanceToConn(NetworkConnection conn) {
        if (conn != null && conn.identity != null) {
            return Vector3.Distance(conn.identity.transform.position, transform.position);
        } else {
            //Debug.LogErrorFormat("Error connection or identity '{0}' is null", conn);
            return -1;
        }
    }

    /// <summary>
    /// Used to find the distance to a Network connection of the local playe to the attached objectr.
    /// <para>The distance from the local player to the attached object is returned.</para>
    /// </summary>
    public float DistanceToLocalConn() {

        return DistanceToConn(LocalClientConn);
    }
}