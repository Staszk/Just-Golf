using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProcessorManager")]
public class NetworkRespawnManager : MonoBehaviour
{
    private int _myID;
    private NetworkManager network = null;

    private void Start()
    {
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();

    }

    /// <summary>
    /// Respawn a player to the correct position on the network. 
    /// </summary>
    /// <param name="splitCode">The message data from the packets</param>
    internal void UpdateRespawn(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        Vector3 pos = SerializationScript.StringToVector3(splitCode[2]);
        GameObject player = network.GetPlayerOfID(id);

        if (id == _myID) { RespawnManager.instance.Revive(player.GetComponent<PlayerHealth>(), pos); }
        else { RespawnManager.instance.Revive(player, pos); }
    }
}
