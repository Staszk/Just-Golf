using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationScript
{
    #region STRUCTS/CLASSES/ENUMS

    [System.Serializable]
    public struct BulletRayCast{
        public List<Vector3> shootPosition;
        public List<Vector3> shootDirection;
        public int id;
        public float range;
        public float damage;
        public bool isZoomed;
    }

    [System.Serializable]
    public struct ScoreUpdate
    {
        public int ID;
        public int CurrentStrokes;
        public int? PersonalBest;
        public int Kills;
        public int Deaths;
    }

    public struct BallShot
    {
        public int ballID;
        public int shooterID;
        public Vector3 direction;
        public bool isAbilityBall;
        public bool shouldHome;
        public int homingTargetID;
    }

    #endregion

    #region SERIALIZATION

    public static string SerlizalizePersonalScore(int id, PersonalScoreManager.TYPE type) { return GameEvent.PERSONAL_SCORE + ":" + id + ":" + (int)type; }

    public static string SerlizalizeGolfSwingIndex(int id, int golfIndex) { return GameEvent.ANIMATION + ":" + GameEvent.GOLF_SWING_ANIM + ":" + id + ":" + golfIndex; }

    public static string SerlializeModeChange(int id, bool isGolf) { return GameEvent.ANIMATION + ":" + GameEvent.MODE_CHANGE + ":" + id + ":" + isGolf; }

    public static string SerlializeJumpMessage(int id) { return GameEvent.ANIMATION + ":" + GameEvent.JUMP + ":" + id; }

    public static string SerlializeRunAnimationState(int id, PlayerAnimation.RunState state) { return GameEvent.ANIMATION + ":" + GameEvent.RUN_ANIM + ":" + id + ":" + (int)state; }

    public static string SerlializeShieldMessage(int id) { return GameEvent.SHIELD.ToString() + ":" + id; }

    public static string SerlializeChangeHoleEvent() { return GameEvent.CHANGE_HOLE.ToString(); }

    public static string SerializeTeamScore(int team) { return GameEvent.TEAM_SCORE + ":" + team; }

    public static string SerializeScore(int id, int currentStrokes, int? bestStrokes, int kills, int deaths)
    {
        string endString = GameEvent.SCORE + ":" + id + ":" + currentStrokes + ":" + bestStrokes + ":" + kills + ":" + deaths;
        return endString;
    }

    /// <summary>
    /// Serializes the transform into a string. ObjectType_TRANSFOPM:itemNum:x,x,x:...:
    /// </summary>
    /// <param name="itemNum"> Index of the transform | 1 - Position | 2 - Rotation | 3 - Scale | Combinations can be done 
    /// </param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static string SerializeTransform(string itemNum, Transform transform, GameEvent objectType, bool isAbilityBall = false)
    {
        string endString = objectType + ":" + itemNum + ":";
        int size = itemNum.Length;
        int num;
        for (int i = 0; i < size; i++)
        {
            if (!int.TryParse(itemNum[i].ToString(), out num))
            {
                Debug.LogError("ItemNum needs to be a number!");
                return "";
            }

            switch (num)
            {
                case 1:
                    endString += "POS:" + Vector3ToString(transform.position) + ":";
                    break;
                case 2:
                    endString += "ROT:" + Vector3ToString(transform.rotation.eulerAngles) + ":";
                    break;
                case 3:
                    endString += "SCL:" + Vector3ToString(transform.localScale) + ":";
                    break;
                default:
                    Debug.LogError("ItemNum not valid");
                    break;
            }
        }
        endString += isAbilityBall;
        return endString;
    }

    public static string SerializeRayCast(List<Vector3> shootPosition, List<Vector3> shootDirection, float range, float damage, int id, bool isZoomed)
    {
        int size = shootPosition.Count;
        if (shootDirection.Count != size)
        {
            Debug.LogError("THE LISTS DON'T MATCH");
            return "";
        }
        string data = GameEvent.SHOOT + ":" + id + ":" + size + ":";

        for (int i = 0; i < size; i++)
        {
            data += Vector3ToString(shootPosition[i]) + "|";
            data += Vector3ToString(shootDirection[i]) + ":";
        }
        data += range.ToString() + ":" + damage.ToString() + ":" + isZoomed;
        return data;
    }

    public static string SerializeBallMessage(Vector3 direction, int ballID, int myID, bool isAbilityBall, bool shouldHome, int homingTargetID) {return GameEvent.BALL_SHOT + ":" + ballID + ":"  + myID + ":" + Vector3ToString(direction) + ":" + isAbilityBall + ":" + shouldHome + ":" + homingTargetID; }

    public static string SerializeBallVelocityMessage(Vector3 move, int id, bool isAbilityBall = false) {return GameEvent.BALL_MOVEMENT + ":" + id + ":" + Vector3ToString(move) + ":" + isAbilityBall; }

    public static string SerializePlayerHealth(int id, float currentHealth, int shooterID){return GameEvent.HEALTH + ":" + id + ":" + currentHealth + ":" + shooterID;}

    public static string SerializeSpawnMessage(int id, int team){return GameEvent.SPAWN + ":" + id + ":" + team;}

    public static string SerializePlayerVelocity(int id, Vector3 move){return GameEvent.MOVE + ":" + id + ":" + Vector3ToString(move);}

    public static string SerializePoseChange(int id){return GameEvent.POSE_CHANGE.ToString() + ":" + id;}

    public static string SerializeKillFeed(int killerId, int victimID) { return GameEvent.KILLFEED + ":" + killerId + ":" + victimID; }

    public static string SerializeRespawnMessage(Vector3 pos, int id) { return GameEvent.RESPAWN + ":" + id + ":" + Vector3ToString(pos); }

    public static string SerializeEndGameMessage() { return GameEvent.ENDGAME.ToString(); }

    public static string SerializeReadyUpMessage(int id) { return GameEvent.READYUP.ToString() + ":" + id; }

    public static string SerializeRestartGameMessage() { return GameEvent.RESTART_GAME.ToString(); }

    public static string SerializeHealGameMessage(int id, float healAmount, int boxID = -1) {
        string s = GameEvent.HEAL + ":" + id + ":" + healAmount;
        if (boxID >= 0) { s += ":" + boxID; }

        return s;
    }

    public static string SerializeItemGameMessage(int id, ItemBoxController.Items itemID, List<float> floatInfo, List<int> intInfo)
    {
        string s = GameEvent.ITEM + ":" + id + ":" + itemID + ":";

        if (floatInfo != null)
        {
            for(int i = 0; i < floatInfo.Count; i++)
            {
                s += floatInfo[i] + ":";
            }
        }

        if (intInfo != null)
        {
            for (int i = 0; i < intInfo.Count; i++)
            {
                s += intInfo[i] + ":";
            }
        }

        return s;
    }

    public static string SerializeItemBoxPickUp(int id, int boxID) { return GameEvent.ITEMBOX + ":" + boxID  + ":" + id; }

    public static string SerializePayRespectMessage(int id) { return GameEvent.RESPECT + ":" + id; }

    public static string SerializeTeamAssignment(List<int> teamData) {
        string s = GameEvent.TEAM_ASSIGNMENT.ToString();

        for(int i = 0; i < teamData.Count; i++)
        {
            s += ":" + i + ":" + teamData[i];
        }
        return s;
    }

    public static string SerializeAbilitySpawn(int index, Vector3 position)
    {
        string s = GameEvent.ABILITY_SPAWN.ToString() + ":" + index + ":" + Vector3ToString(position);
        return s;
    }
    
    public static string SerializeWorldVFX(WorldSpaceVFX.WorldVFXType type, Vector3 position, int id, int idOfBall)
    {
        string s = GameEvent.WOLRD_VFX.ToString() + ":" + type.ToString() + ":" + Vector3ToString(position) + ":" + id + ":" + idOfBall;
        return s;
    }

    public static string SerializeTriggerdAbilityEvent(int id, GameEvent theEvent, object[] list)
    {
        string s = GameEvent.ABILITY_EVENT + ":" + theEvent.ToString() + ":" + id;
        for(int i = 0; i < list.Length; i++)
        {
            s += ":" + list[i];
        }
        return s;
    }

    public static string SerializeAbilityChangeEvent(int id, int flag, int value) { return GameEvent.ABILITY_CHANGE.ToString() + ":" + id + ":" + flag + ":" + value; }
    #endregion

    #region DESERIALIZATION

    public static ScoreUpdate DeserializeScore(string code)
    {
        string[] splitCode = code.Split(':');
        ScoreUpdate score = new ScoreUpdate();
        score.ID = int.Parse(splitCode[1]);
        score.CurrentStrokes = int.Parse(splitCode[2]);
        score.PersonalBest = int.TryParse(splitCode[3], out int test)? (int?)test : null;      
        score.Kills = int.Parse(splitCode[4]);
        score.Deaths = int.Parse(splitCode[5]);

        return score;
    }

    public static BulletRayCast DeserializeRayCast(string code)
    {
        code = code.Replace("(", "");
        code = code.Replace(")", "");
        string[] splitCode = code.Split(':');

        BulletRayCast ray = new BulletRayCast();
        ray.shootPosition = new List<Vector3>();
        ray.shootDirection = new List<Vector3>();
        ray.id = int.Parse(splitCode[1]);

        int size = int.Parse(splitCode[2]);

        for (int i = 0; i < size; i++)
        {
            string[] s = splitCode[3 + i].Split('|');
            ray.shootPosition.Add(StringToVector3(s[0]));
            ray.shootDirection.Add(StringToVector3(s[1]));
        }
        ray.range = float.Parse(splitCode[splitCode.Length - 3]);
        ray.damage = float.Parse(splitCode[splitCode.Length - 2]);
        ray.isZoomed = bool.Parse(splitCode[splitCode.Length - 1]);

        return ray;
    }

    /// <summary>
    /// Deserializes the code into a Matrix4x4
    /// </summary>
    /// <param name="code"></param>
    /// <param name="itemNum"> Index of the transform 1 - Position | 2 - Rotation | 3 - Scale | Combinations can be done  </param>
    /// <returns></returns>
    public static Matrix4x4 DeserializeTransform(string code, ref string itemNum)
    {
        Vector3 pos = Vector3.zero;
        Vector4 rot = Vector4.zero;
        Vector3 scale = Vector3.zero;

        string[] splitCode = code.Split(':');
        itemNum = splitCode[1];

        for (int i = 2; i < splitCode.Length; i += 2)
        {
            switch (splitCode[i])
            {
                case "POS":
                    pos = StringToVector3(splitCode[i + 1]);
                    break;
                case "ROT":
                    Quaternion quat = Quaternion.Euler(StringToVector3(splitCode[i + 1]));
                    rot = new Vector4(quat.x, quat.y, quat.z, quat.w);
                    break;
                case "SCL":
                    scale = StringToVector3(splitCode[i + 1]);
                    Debug.Log("scale");
                    break;
            }
        }

        Matrix4x4 _transform = new Matrix4x4();// = Matrix4x4.TRS(pos, rot, scale);

        _transform.SetRow(0, pos);
        _transform.SetRow(1, rot);
        _transform.SetRow(2, scale);

        return _transform;
    }

    public static BallShot DeserializeBallShot(string code)
    {
        BallShot shot = new BallShot();
        string[] splitCode = code.Split(':');
        shot.ballID = int.Parse(splitCode[1]);
        shot.shooterID = int.Parse(splitCode[2]);
        shot.direction = StringToVector3(splitCode[3]);
        shot.isAbilityBall = bool.Parse(splitCode[4]);
        shot.shouldHome = bool.Parse(splitCode[5]);
        shot.homingTargetID = int.Parse(splitCode[6]);
        return shot;
    }

    #endregion

    #region HELPER_FUNCTIONS

    public static Vector3 StringToVector3(string vector)
    {
        Vector3 final;
        float result;
        string[] array = vector.Split(',');

        if (!float.TryParse(array[0], out result))
        {
            Debug.LogError("Invalid vector format");
            return Vector3.zero;
        }
        final.x = result;


        if (!float.TryParse(array[1], out result))
        {
            Debug.LogError("Invalid vector format");
            return Vector3.zero;
        }

        final.y = result;

        if (!float.TryParse(array[2], out result))
        {
            Debug.LogError("Invalid vector format");
            return Vector3.zero;
        }

        final.z = result;

        return final;
    }

    public static string Vector3ToString(Vector3 vector)
    {
        return vector.x + "," + vector.y + "," + vector.z;
    }

    #endregion

}
