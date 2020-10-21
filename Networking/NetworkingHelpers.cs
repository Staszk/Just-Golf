using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region STRUCTS/ENUMS/CLASSES
public enum GameEvent
{
    DEFAULT = 0,
    PLAYER_MOVEMENT,
    BALL_MOVEMENT,
    PLAYER_TRANSFORM,
    BALL_TRANSFORM,
    SPAWN,
    SHOOT,
    BALL_SHOT,
    MOVE,
    SCORE,
    ITEM,   //Use Item
    POSE_CHANGE,
    HEALTH,
    KILLFEED,
    RESPAWN,
    ENDGAME,
    READYUP,
    RESTART_GAME,
    HEAL,
    ITEMBOX,
    RESPECT, 
    TEAM_ASSIGNMENT, 
    TEAM_REQUEST, 
    ABILITY_SPAWN, 
    WOLRD_VFX, 
    FREEZE, 
    TEAM_SCORE, 
    ABILITY_EVENT,
    ABILITY_EVENT_FREEZE, 
    ABILITY_EVENT_AMP, 
    CHANGE_HOLE, 
    SHIELD, 
    ABILITY_CHANGE, 
    RUN_ANIM, 
    ANIMATION, 
    JUMP, 
    MODE_CHANGE, 
    GOLF_SWING_ANIM, 
    PERSONAL_SCORE
}

[System.Serializable]
struct GolfBallTrails
{
    public Material[] trail;
}
#endregion