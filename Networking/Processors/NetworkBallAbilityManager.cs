    using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("NetworkManager")]
public class NetworkBallAbilityManager : MonoBehaviour
{

    private NetworkManager network;

    private void Start()
    {
        network = NetworkManager.instance;
    }

    internal void SpawnBall(string[] splitcode)
    {
        int index = int.Parse(splitcode[1]);
        Vector3 position = SerializationScript.StringToVector3(splitcode[2]);
        EventController.FireEvent(new NetworkAbilityBallMessage(index, position));
    }

    internal void AbilityBallVFX(string[] splitcode)
    {
        
        WorldSpaceVFX.WorldVFXType type = (WorldSpaceVFX.WorldVFXType)System.Enum.Parse(typeof(WorldSpaceVFX.WorldVFXType), splitcode[1]);
        Vector3 position = SerializationScript.StringToVector3(splitcode[2]);
        int id = int.Parse(splitcode[3]);
        int ballID = int.Parse(splitcode[4]);

        if(ballID >= 0)
        {
            EventController.FireEvent(new DeactivateBall(ballID));
        }

        if(id > 0)
        {
            EventController.FireEvent(new WorldVFXMessage(type, position,network.GetPlayerOfID(id).transform));
        }
        else
        {
            EventController.FireEvent(new WorldVFXMessage(type, position));
        }

    }

    internal void DoAbilityEvent(string[] splitcode, int delay)
    {
        float delayInSeconds = ((float)delay / 1000.0f);
        int id = int.Parse(splitcode[2]);

        if(splitcode[1] == GameEvent.ABILITY_EVENT_FREEZE.ToString())
        {
            float duration = float.Parse(splitcode[3]) - delayInSeconds;

            if (id == network.GetId())
            {       
                EventController.FireEvent(new NetworkFreeze(duration));
            }

        }else if(splitcode[1] == GameEvent.ABILITY_EVENT_AMP.ToString())
        {
            if(id == network.GetId())
            {
                float speed = float.Parse(splitcode[3]);
                int damage = int.Parse(splitcode[4]);
                float range = float.Parse(splitcode[5]);
                float effectLength = float.Parse(splitcode[6]) - delayInSeconds;
                EventController.FireEvent(new AMPBallMessage(id, speed, damage, range));
                StartCoroutine(EndEffect(id, effectLength));
            }
            
        }     
    } 
    IEnumerator EndEffect(int id, float effectLength)
    {
        yield return new WaitForSeconds(effectLength);
        EventController.FireEvent(new EndAMPBallMessage(id));
    }
}
