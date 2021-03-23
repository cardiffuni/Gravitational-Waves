using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Minimap : MonoBehaviour
{
    public NetworkConnectionToClient LocalClientConn { get => NetworkServer.localConnection; }
    public Transform LocalClientTransform { get => LocalClientConn.identity.transform; }

    private void LateUpdate()
    {
        Vector3 newPosition = LocalClientTransform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, LocalClientTransform.eulerAngles.y, 0F);
    }
}
