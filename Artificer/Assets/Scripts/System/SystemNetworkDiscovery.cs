using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Artificer
using Data.UI;


/// <summary>
/// Custom build of network discovery 
/// for artificer
/// Listens for server then dispatches event
/// when one is found
/// </summary>
public class SystemNetworkDiscovery : NetworkDiscovery
{
    #region EVENTS

    public delegate void DiscoveryEvent(string serverIP, string serverName);

    public event DiscoveryEvent OnServerDiscovery;

    #endregion

    #region NETWORKDISCOVERY OVERRIDE

    // Raises the received broadcast event.
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (OnServerDiscovery != null)
            OnServerDiscovery(fromAddress, data);
    }

    #endregion
}
