using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkResponseMessage : NetworkMessage
{
    public int NumberOfPlayers { get; set; }
    // The server that sent this
    // this is a property so that it is not serialized,  but the
    // client fills this up after we receive it
    public IPEndPoint EndPoint { get; set; }

    public Uri uri;
    public string gameCode;

    // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
    public long serverId;
}
