using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class NetworkEventsDispatcher : MonoBehaviour
{
    ///
    /// DISPATCH NETWORK EVENTS MORE EASILY
    ///

    public static event Action OnConnectFailed;
    public static event Action OnPlayerJoined;
    public static event Action OnPlayerLeft;


    public void ConnectFailed(NetworkRunner runner, NetAddress address, NetConnectFailedReason reason)
    {
        if (OnConnectFailed != null) OnConnectFailed.Invoke();
    }
     public void PlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (OnPlayerJoined != null) OnPlayerJoined.Invoke();
    }
    public void PlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (OnPlayerLeft != null) OnPlayerLeft.Invoke();
    }

}
