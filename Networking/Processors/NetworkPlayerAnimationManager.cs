using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("NetworkManager")]
public class NetworkPlayerAnimationManager : MonoBehaviour
{
    internal void ProcessAnimation(string[] splitCode, float delay)
    {
        float delayInSeconds = ((float)delay / 1000.0f);
        string type = splitCode[1];

        if (type == GameEvent.RUN_ANIM.ToString()) { ProcessRunAnimation(splitCode); }
        else if (type == GameEvent.JUMP.ToString()) { ProcessJumpAnimation(splitCode, delayInSeconds); }
        else if (type == GameEvent.MODE_CHANGE.ToString()) { ProcessModeChange(splitCode, delayInSeconds); }
        else if (type == GameEvent.GOLF_SWING_ANIM.ToString()) { ProcessClubSwing(splitCode, delayInSeconds); }
    }

    private void ProcessRunAnimation(string[] splitCode)
    {
        int id = int.Parse( splitCode[2]);
        int state = (int.Parse(splitCode[3]));
        EventController.FireEvent(new EnemyRunAnimation(id, state));
    }
        
    private void ProcessJumpAnimation(string[] splitCode, float delay)
    {
        int id = int.Parse(splitCode[2]);
        EventController.FireEvent(new EnemyJump(id, delay));
    }

    private void ProcessModeChange(string[] splitCode, float delay)
    {
        int id = int.Parse(splitCode[2]);
        bool isGolfMode = bool.Parse(splitCode[3]);
        EventController.FireEvent(new EnemyModeChange(id, isGolfMode));
    }

    private void ProcessClubSwing(string[] splitCode, float delay)
    {
        int id = int.Parse(splitCode[2]);
        int index = int.Parse(splitCode[3]);
        EventController.FireEvent(new EnemyClubSwingAnim(id, delay, index));
    }

}
