using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProcessorManager")]
public class NetworkItemManager : MonoBehaviour
{
    public static event System.Action<float> EventInk = delegate { };
    public static event System.Action EventConfused = delegate { };
    public static event System.Action<NetworkBoxPickup> EventBoxPickup = delegate { };
    public static event System.Action<ItemFeedMessage> EventItemFeedEvent = delegate { };
    public static event System.Action<int> EventHealthPickUp = delegate { };
    public static event Action<ParticleEffectMessage> EventClientParticleEffect = delegate { };

    #region PLUGIN
    [DllImport("egp-net-plugin-Unity")] static extern int getNetworkID();
    [DllImport("egp-net-plugin-Unity")] static extern void deleteNetwork();
    [DllImport("egp-net-plugin-Unity")] static extern void updateNetwork();
    [DllImport("egp-net-plugin-Unity")] static extern int SendData(string data, int PriorityNum, bool sendAll, int id = -1);
    [DllImport("egp-net-plugin-Unity")] static extern bool hasPriorityMessage();
    [DllImport("egp-net-plugin-Unity")] static extern bool hasNormalMessage();
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr getPriorityMessage(ref int id, ref int delay);
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr getNormalMessage(ref int id);
    #endregion

    private int _myID;
    private NetworkManager network = null;     

    private void Start()
    {
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();       
    }

    /// <summary>
    /// Sorts all of the item messages and calls the correct event 
    /// </summary>
    /// <param name="splitCode">The message data from the packets</param>
    /// <param name="delay"> The delay between sending and recieving packets </param>
    internal void ProcessItem(string[] splitCode, int delay = 0)
    {
        int id = int.Parse(splitCode[1]);

        if (splitCode[2] == ItemBoxController.Items.SwingFuel.ToString())
        {
            if (id != _myID)
            {
                float duration = float.Parse(splitCode[3]);
                EventInk(duration);
            }

            EventItemFeedEvent(new ItemFeedMessage(id, ItemBoxController.Items.SwingFuel));
            EventClientParticleEffect(new ParticleEffectMessage(ParticleEffectMessage.Effect.Inked, id));
        }
        else if (splitCode[2] == ItemBoxController.Items.WaterBottle.ToString())
        {
            float health = float.Parse(splitCode[3]);
            HealthPlayer(id, health);
            network.SendHealRequest(health, id);

            EventItemFeedEvent(new ItemFeedMessage(id, ItemBoxController.Items.WaterBottle));
        }
        else if (splitCode[2] == ItemBoxController.Items.RangeFinder.ToString())
        {
            //UI/KillFeedUpdate
            EventItemFeedEvent(new ItemFeedMessage(id, ItemBoxController.Items.RangeFinder));
        }
        else if (splitCode[2] == ItemBoxController.Items.ConfusedControls.ToString())
        {
            if (id != _myID)
            {
                float duration = float.Parse(splitCode[3]);
                EventConfused();
                StartCoroutine(ToggleConfuse(duration));
            }

            EventItemFeedEvent(new ItemFeedMessage(id, ItemBoxController.Items.ConfusedControls));
            EventClientParticleEffect(new ParticleEffectMessage(ParticleEffectMessage.Effect.Confused, id));
        }
        else
        {
            Debug.LogError("< " + splitCode[2] + " >ITEMS NOT PROCESSED");
        }
    }

    /// <summary>
    /// Update a players health based on a health pack (host)
    /// </summary>
    /// <param name="splitCode">The message data from the packets</param>
    internal void HealthPlayer(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        float health = float.Parse(splitCode[2]);

        IHaveHealth target = network.GetPlayerOfID(id).GetComponent(typeof(IHaveHealth)) as IHaveHealth;
        target.GainHealth(health);

        if (splitCode.Length >= 4)
        {
            int boxID = int.Parse(splitCode[3]);
            EventHealthPickUp(boxID);
        }
    }

    /// <summary>
    /// Update the health of a player based on a health pack (client)
    /// </summary>
    /// <param name="id"> ID of the player to heal </param>
    /// <param name="health"> The amount to heal the player. </param>
    internal void HealthPlayer(int id, float health)
    {
        IHaveHealth target = network.GetPlayerOfID(id).GetComponent(typeof(IHaveHealth)) as IHaveHealth;
        target.GainHealth(health);
    }


    /// <summary>
    /// Responsible for sending the item used and updating the particle effects on the effected players
    /// </summary>
    /// <param name="itemID"> ID of the item used </param>
    /// <param name="floatInfo"> Floats needed for the item </param>
    /// <param name="intInfo"> Ints needed for the item </param>
    /// <param name="sendToAll"> Should send to all players (false = just host)</param>
    public void SendItemUseRequest(ItemBoxController.Items itemID, List<float> floatInfo, List<int> intInfo, bool sendToAll = true)
    {
        string s = SerializationScript.SerializeItemGameMessage(_myID, itemID, floatInfo, intInfo);
        SendData(s, 1, sendToAll);
        if (network.IsHost()) { ProcessItem(s.Split(':')); }
        else
        {
            EventItemFeedEvent(new ItemFeedMessage(_myID, itemID));

            switch (itemID)
            {
                case ItemBoxController.Items.SwingFuel:
                    EventClientParticleEffect(new ParticleEffectMessage(ParticleEffectMessage.Effect.Inked, _myID));
                    break;
                case ItemBoxController.Items.ConfusedControls:
                    EventClientParticleEffect(new ParticleEffectMessage(ParticleEffectMessage.Effect.Confused, _myID));
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Calls the Event box pickup event to toggle the item boxes on all screens.
    /// </summary>
    /// <param name="splitCpde">The message data from the packets</param>
    internal void UpdateItemBox(string[] splitCpde)
    {
        int boxID = int.Parse(splitCpde[1]);
        int id = int.Parse(splitCpde[2]);

        EventBoxPickup(new NetworkBoxPickup(boxID, id == _myID));
    }

    /// <summary>
    /// Toggle the confused event on the local player
    /// </summary>
    /// <param name="delay">How long to be confused for </param>
    /// <returns></returns>
    IEnumerator ToggleConfuse(float delay)
    {
        yield return new WaitForSeconds(delay);
        EventConfused();
    }
}
