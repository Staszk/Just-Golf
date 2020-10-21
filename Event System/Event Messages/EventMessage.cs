using UnityEngine;

public class EventMessage
{
    public EventMessage() {}
}

public sealed class GameStartMessage : EventMessage { }

public sealed class GameTimeMessage : EventMessage
{
    public float totalGameTime;
    public float timeSpentInGame;
    public float percentDone;
    public float timeToDisplay;


    public GameTimeMessage (float t, float s, float ttd)
    {
        totalGameTime = t;
        timeSpentInGame = s;
        timeToDisplay = ttd;

        percentDone = (timeSpentInGame / totalGameTime);
    }
}

public sealed class AMPBallMessage : EventMessage
{
    public int playerID;
    public float speedMultiplier;
    public int damageMultiplier;
    public float rangeMultiplier;

    public AMPBallMessage(int id, float speed, int damage, float range)
    {
        playerID = id;
        speedMultiplier = speed;
        damageMultiplier = damage;
        rangeMultiplier = range;
    }
}

public sealed class EndAMPBallMessage : EventMessage
{
    public int playerID;

    public EndAMPBallMessage(int id)
    {
        playerID = id;
    }
}

public sealed class StartIKMessage : EventMessage
{

}

public sealed class StopIKMessage : EventMessage
{
}

public sealed class TrackSuperlativeMessage : EventMessage
{
    public SuperlativeController.Superlative superlative;
    public SuperlativeController.ConditionFlag condition;
    public float value;
    public int id;

    public TrackSuperlativeMessage(SuperlativeController.Superlative s, SuperlativeController.ConditionFlag c, float v, int id = -1)
    {
        superlative = s;
        condition = c;
        value = v;
        this.id = id;
    }
}

public sealed class NetworkedSuperlativeMessage : EventMessage
{
    public TrackSuperlativeMessage message;

    public NetworkedSuperlativeMessage(TrackSuperlativeMessage m)
    {
        message = m;
    }
}

public sealed class SwingAnimationMessage : EventMessage
{

}

public sealed class SwingClubMessage: EventMessage
{

}

public sealed class MakeSureThePlayerCanMoveMessage : EventMessage
{

}


public sealed class PlayerHealthChangeMessage : EventMessage
{
    public int currentHealth;

    public PlayerHealthChangeMessage(int h)
    {
        currentHealth = h;
    }
}

public sealed class TeamScoreMessage : EventMessage
{
  
}

public sealed class EnemyScoreMessage : EventMessage
{

}

public sealed class GameTimerMessage : EventMessage 
{

}

public sealed class SettingsMenuToggleMessage : EventMessage 
{
	public bool toggledOn;

	public SettingsMenuToggleMessage(bool on)
	{
		toggledOn = on;
	}
}

public sealed class ControlRebindMessage : EventMessage
{
    public GameControls control;

    public ControlRebindMessage(GameControls gc)
    {
        control = gc;
    }
}

public sealed class UpdateControlsMessage : EventMessage { }

public sealed class SimulatedDamageMessage : EventMessage
{
    public Vector3 worldPosition;
    public float damageAmount;
    public bool isCritical;

    public SimulatedDamageMessage(Vector3 wp, float d, bool c)
    {
        worldPosition = wp;
        damageAmount = d;
        isCritical = c;
    }
}

public sealed class ObjectBallsSpawnedMessage : EventMessage
{
    public ColoredBall[] ballsArray;

    public ObjectBallsSpawnedMessage(ColoredBall[] balls)
    {
        ballsArray = balls;
    }
}

public sealed class AtFullHealthMessage : EventMessage
{

}

public sealed class GolfChargeMessage : EventMessage
{
    public float amount;

    public GolfChargeMessage(float amnt)
    {
        amount = amnt;
    }
}

public sealed class AbilityCooldownMessage : EventMessage
{
    public int ability;
    public float amount;

    public AbilityCooldownMessage(int ability, float amount)
    {
        this.ability = ability;
        this.amount = amount;
    }
}

public sealed class NearbyGolfBallMessage : EventMessage
{
    public GolfBall golfBall;

    public NearbyGolfBallMessage(GolfBall go)
    {
        golfBall = go;
    }
}

public sealed class ModeChangedMessage : EventMessage
{
    public bool modeChangedToGolf;

    public ModeChangedMessage(bool golfMode)
    {
        modeChangedToGolf = golfMode;
    }
}

public sealed class AbilityBallPrepareMessage : EventMessage
{
    public AbilityBall.Abilities ability;
    public PlayerController player;
    public int slotID;

    public AbilityBallPrepareMessage(PlayerController p, AbilityBall.Abilities ab, int id)
    {
        ability = ab;
        player = p;
        slotID = id;
    }
}

public sealed class AbilityBallUseMessage : EventMessage
{
    public int slotID;

    public AbilityBallUseMessage(int id)
    {
        slotID = id;
    }
}

public sealed class InitializeAbilityBall : EventMessage
{
    public AbilityBall.Abilities ability;
    public int id;

    public InitializeAbilityBall(AbilityBall.Abilities ab, int id)
    {
        ability = ab;
        this.id = id;
    }
}

public sealed class JumpMessage : EventMessage {}

public sealed class ClubPowerChangedMessage : EventMessage
{
    public float power;
    public bool isInit;

    public ClubPowerChangedMessage(float p, bool isInit = false)
    {
        power = p;
        this.isInit = isInit;
    }
}

public sealed class ClubChangedMessage : EventMessage
{
    public int index;
    public ClubStat stats;

    public ClubChangedMessage(int index, ClubStat club)
    {
        this.index = index;
        stats = club;
    }
}

public sealed class GolfStrokeMessage : EventMessage
{
    public float power;

    public GolfStrokeMessage (float p)
    {
        power = p;
    }
}

public sealed class CameraTransitionMessage : EventMessage
{

}

public sealed class FOVChangeMessage : EventMessage
{
    public float zoomSpeed;
    public bool loweringFOV;
    public int zoomLayer;

    public FOVChangeMessage(float speed, bool lowering, int layer)
    {
        zoomSpeed = speed;
        loweringFOV = lowering;
        zoomLayer = layer;
    }
}

public sealed class WorldVFXMessage : EventMessage
{
    public WorldSpaceVFX.WorldVFXType type;
    public Vector3 worldPosition;
    public Transform parent; // null if no parent :: IN FUTURE, THIS IS THE ID OF NETWORKED PLAYER

    public WorldVFXMessage (WorldSpaceVFX.WorldVFXType t, Vector3 pos, Transform p = null)
    {
        type = t;
        worldPosition = pos;
        parent = null;
    }

}

public sealed class ParticleEffectMessage : EventMessage
{
    public enum Effect { Inked, Confused };

    public Effect effectType;
    public int IdOfUser = -1;

    public ParticleEffectMessage (Effect type, int id)
    {
        effectType = type;
        IdOfUser = id;
    }
}

public sealed class ItemFeedMessage : EventMessage
{
    public int userID;
    public ItemBoxController.Items item;

    public ItemFeedMessage(int id, ItemBoxController.Items new_Item)
    {
        userID = id;
        this.item = new_Item;
    }
}

public sealed class ClientDeathMessage : EventMessage
{
    public int killerID;

    public ClientDeathMessage (int killerID)
    {
        this.killerID = killerID;
    }
}

public sealed class ClientWaitToRespawnMessage : EventMessage
{
    public float timeLeft;

    public ClientWaitToRespawnMessage(float t)
    {
        timeLeft = t;
    }
}


public sealed class ClientRespawnMessage : EventMessage
{

}

public sealed class NameMessage: EventMessage
{
    public System.Action<string, GameObject> action;
    public string title;

    public NameMessage(System.Action<string, GameObject> action, string title)
    {
        this.action = action;
        this.title = title;
    }
}

public sealed class PersonalScoreMessage : EventMessage
{
    public int id;
    public PersonalScoreManager.TYPE type;

    public PersonalScoreMessage(int id, PersonalScoreManager.TYPE type)
    {
        this.id = id;
        this.type = type;
    }
}

#region Network Message

public sealed class NetworkPersonalScoreMessage : EventMessage
{
    public int id;
    public PersonalScoreManager.TYPE type;

    public NetworkPersonalScoreMessage(int id, PersonalScoreManager.TYPE type)
    {
        this.id = id;
        this.type = type;
    }
}

public sealed class EnemyClubSwingAnim : EventMessage
{
    public int activeClub;
    public float delay;
    public int id;
    public EnemyClubSwingAnim(int id, float delay, int activeClub)
    {
        this.activeClub = activeClub;
        this.id = id;
        this.delay = delay;
    }
}

public sealed class SendClubSwingAnimation : EventMessage
{
    public int activeClub;

    public SendClubSwingAnimation(int activeClub)
    {
        this.activeClub = activeClub;
    }
}

public sealed class EnemyModeChange : EventMessage
{
    public int id;
    public bool isGolfMode;

    public EnemyModeChange(int id, bool isGolfMode)
    {
        this.id = id;
        this.isGolfMode = isGolfMode;
    }
}

public sealed class EnemyJump : EventMessage
{
    public int id;
    public float delay;

    public EnemyJump(int id, float delay)
    {
        this.id = id;
        this.delay = delay;
    }
}


public sealed class EnemyRunAnimation : EventMessage
{
    public int id;
    public int state;

    public EnemyRunAnimation(int id, int state)
    {
        this.id = id;
        this.state = state;
    }
} 
public sealed class SendRunAnimMessage : EventMessage
{
    public PlayerAnimation.RunState state;

    public SendRunAnimMessage(PlayerAnimation.RunState state)
    {
        this.state = state;
    }
}
public sealed class SendAbilityBallUpdate : EventMessage
{
    public int flag;
    public int id;
    public int value;

    public SendAbilityBallUpdate(int flag, int id, int value)
    {
        this.flag = flag;
        this.id = id;
        this.value = value;
    }
}
public sealed class SendShieldMessage : EventMessage
{
    public int id;

    public SendShieldMessage(int id)
    {
        this.id = id;
    } 
}

public sealed class ChangeHoleEvent : EventMessage { }

public sealed class DeactivateBall : EventMessage
{
    public int id;

    public DeactivateBall(int id)
    {
        this.id = id;
    }
}


public sealed class LobbyStateChange : EventMessage
{
  
}

public sealed class LobbyOpened : EventMessage
{
    public int index;
    public string name;

    public LobbyOpened(int index, string name)
    {
        this.index = index;
        this.name = name;
    }
}

public sealed class OpenWarningMessage : EventMessage
{
    public string msg;

    public OpenWarningMessage(string msg)
    {
        this.msg = msg;
    }
}

public sealed class IDMessage : EventMessage
{
    public int id;

    public IDMessage(int id) {
        this.id = id;
    }
}

public sealed class CloseWarningMessage : EventMessage
{
}

public sealed class EnableCurrentPanel : EventMessage
{
}


public sealed class StartLobby : EventMessage { }

public sealed class JoinLobby : EventMessage
{
    public int index;

    public JoinLobby(int index)
    {
        this.index = index;
    }
}

public sealed class ConnectedUsersUpdate : EventMessage
{
    public System.Collections.Generic.List<int> teams;

    public ConnectedUsersUpdate(System.Collections.Generic.List<int> teams)
    {
        this.teams = teams;
    }
}


public sealed class NetworkedPlayerFireMessage : EventMessage
{
    public int id;
    public Vector3 pos;

    public NetworkedPlayerFireMessage(int id, Vector3 pos)
    {
        this.id = id;
        this.pos = pos;
    }
}

public sealed class NetworkKillFeedMessage : EventMessage
{
    public int killerID, victimID;

    public NetworkKillFeedMessage(int killer, int victim)
    {
        killerID = killer;
        victimID = victim;
    }
}

public sealed class NetworkPayRespectsMessage : EventMessage
{
    public int playerID;

    public NetworkPayRespectsMessage(int id)
    {
        playerID = id;
    }
}

public sealed class NetworkReadyUp : EventMessage
{
    public int id;

    public NetworkReadyUp(int newId)
    {
        id = newId;
    }
}

public sealed class NetworkBoxPickup : EventMessage
{
    public int boxId;
    public bool isMine;

    public NetworkBoxPickup(int boxId, bool isMine)
    {
        this.boxId = boxId;
        this.isMine = isMine;
    }
}

public sealed class NetworkAbilityBallMessage : EventMessage
{
    public int index;
    public Vector3 position;

    public NetworkAbilityBallMessage(int index, Vector3 position)
    {
        this.index = index;
        this.position = position;
    }
}

public sealed class NetworkFreeze : EventMessage
{
    public float duration;

    public NetworkFreeze(float duration)
    {
        this.duration = duration;
    }
}

public sealed class NetworkSendBallRespawn : EventMessage
{
    public GolfBall ball;
    public int teamScorer;

    public NetworkSendBallRespawn(GolfBall ball, int teamScorer)
    {
        this.ball = ball;
        this.teamScorer = teamScorer;
    }
}

public sealed class SetTeamUIMessage : EventMessage
{
    public Transform teammate;

    public SetTeamUIMessage(Transform teammate)
    {
        this.teammate = teammate;
    }
}

public sealed class EndGameMessage : EventMessage
{

}

public sealed class VictoryDefeatMessage : EventMessage
{
    public bool isVictorious;

    public VictoryDefeatMessage(bool _isVictorious)
    {
        isVictorious = _isVictorious;
    }

}

public sealed class ShowEndScreenMessage : EventMessage
{

}

public sealed class AssignSuperlativeMessage : EventMessage
{
    public SuperlativeController.Superlative yourSuperlative;
    public SuperlativeController.Superlative teammateSuperlative;
    public SuperlativeController.Superlative enemy1Superlative;
    public SuperlativeController.Superlative enemy2Superlative;

    public AssignSuperlativeMessage(SuperlativeController.Superlative yours, SuperlativeController.Superlative teammates, 
        SuperlativeController.Superlative enemy1, SuperlativeController.Superlative enemy2)
    {
        yourSuperlative = yours;
        teammateSuperlative = teammates;
        enemy1Superlative = enemy1;
        enemy2Superlative = enemy2;
    }
}

#endregion
