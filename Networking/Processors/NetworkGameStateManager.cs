using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProcessorManager")]
public class NetworkGameStateManager : MonoBehaviour
{
    public static event System.Action EventEndGame = delegate { };
    public static event System.Action<NetworkKillFeedMessage> EventKillFeed = delegate { };
    public static event System.Action<NetworkReadyUp> EventReady = delegate { };
    public static event System.Action<NetworkPayRespectsMessage> EventPayRespects = delegate { };

    public GameObject _characterPrefab;

    private int _myID;
    private NetworkManager network = null;

    internal void CallEndGameEvent() { EventEndGame(); }
    internal void CallReadyEvent(NetworkReadyUp action) { EventReady(action); }
   
    private HoleManager holeManager;

    private void Start()
    {        
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();

        holeManager = GameObject.FindGameObjectWithTag("HoleManager")?.GetComponent<HoleManager>();

    }

    /// <summary> Sends out the end game event </summary>
    internal void UpdateEndGame()
    {
        EventEndGame();
    }

    /// <summary>
    /// Sends out the kill feed event to tell users if a player had dies or used 
    /// an item. 
    /// </summary>
    /// <param name="splitCode"> The message data from the packets</param>
    internal void UpdateKillFeed(string[] splitCode)
    {
        int killer = int.Parse(splitCode[1]);
        int victim = int.Parse(splitCode[2]);
        EventKillFeed(new NetworkKillFeedMessage(killer, victim));
        if (victim != _myID && !network.IsHost()) { RespawnManager.instance.Death(network.GetPlayerOfID(victim)); }
    }

    /// <summary>
    /// Updates the scoreboard of a specific player.
    /// </summary>
    /// <param name="code">The message data from the packets</param>
    internal void UpdateScore(string code)
    {
        SerializationScript.ScoreUpdate score = SerializationScript.DeserializeScore(code);
       // network.GetPlayerOfID(score.ID).GetComponent<PlayerScoreTracker>().UpdateEntity(score.ID, score.CurrentStrokes, score.PersonalBest, score.Kills, score.Deaths, score.ID == _myID);
    }

    /// <summary>
    /// Sends out the ready up player event. Players ready up at the end of the game 
    /// to play again (restarting currently does not work with all systems)
    /// </summary>
    /// <param name="splitCode">The message data from the packets</param>
    internal void UpdateReadyPlayers(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        EventReady(new NetworkReadyUp(id));
        network.ReadyAPlayer(id);
        print("Player " + id + " is ready!");
    }

    /// <summary> Reloads the scene to restart the game. (Does not work with all systems.) </summary>
    internal void UpdateRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Calls the pay respects event if the local player is dead. 
    /// </summary>
    /// <param name="splitCode">The message data from the packets</param>
    internal void UpdateRespects(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);

        if (network.GetLocalPlayer().GetComponent<PlayerHealth>().Health <= 0)
        {
            EventPayRespects(new NetworkPayRespectsMessage(id));
        }
    }

    internal void UpdateTeamScore(string[] splitCode)
    {
        int team = int.Parse(splitCode[1]);
        if (team == network.GetMyTeam())
        {
            EventController.FireEvent(new TeamScoreMessage());
        }
        else
        {
            EventController.FireEvent(new EnemyScoreMessage());
        }
    }

    /// <summary>
    /// Spawns a player on the netowrk. Responsible for spawning all things related to 
    /// the player (the ball).
    /// </summary>
    /// <param name="code">The message data from the packets</param>
    internal void SpawnPlayerOnNetwork(string code)
    {
        string[] splitcode = code.Split(':');
        int id = int.Parse(splitcode[1]);
        int team = int.Parse(splitcode[2]);

        GameObject player = network.GetPlayerOfID(id);

        player = Instantiate(_characterPrefab);
        player.name = id.ToString();
        player.GetComponent<PlayerScoreTracker>().Init(id);

        player.GetComponent<EnemyAnimation>().SetMaterial(network.GetMaterialOfPlayer(team));

        if (team == network.GetMyTeam())
        {          
            EventController.FireEvent(new SetTeamUIMessage(player.transform));
        }

        Vector3 location = RespawnManager.instance.GetRespawnLocationAtIndex(id);
        player.GetComponent<EnemyHealth>().Respawn(location);
        player.GetComponent<EnemyHealth>().MyID = id;

        network.AddNumberOfConnectedPlayers();
        network.AddBallAndPlayerToNetwork(player, id);

        AbilityBall.Abilities off = (AbilityBall.Abilities)(NetworkStoredData.instance.GetOffenseAbility(id) + 1);
        AbilityBall.Abilities def = (AbilityBall.Abilities)(NetworkStoredData.instance.GetOffenseAbility(id) + 3);

        EventController.FireEvent(new InitializeAbilityBall(off, id));
        EventController.FireEvent(new InitializeAbilityBall(def, id));
        EventController.FireEvent(new InitializeAbilityBall(AbilityBall.Abilities.storageBall, id));
    }

    internal void ChangeHole()
    {
        if (holeManager)
        {
            holeManager.ChangeHole(false);
        }
        else
        {
            Debug.LogError("Could not find hole manager");
        }
    }

    internal void UpdatePersonalScore(string[] splitCode)
    {
        int id = int.Parse(splitCode[1]);
        PersonalScoreManager.TYPE type = (PersonalScoreManager.TYPE)(int.Parse(splitCode[2]));
        if(id == _myID)
        {
            //TODO add shit
        }
        EventController.FireEvent(new NetworkPersonalScoreMessage(id, type));

    }
}
