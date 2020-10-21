///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            NetworkManager
///   Description:      Sends Packets to the host/clients
///   Author:           Mark Botaish
///   Contributor(s):   Parker Staszkiewicz 
///-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : EventListener
{
    public static NetworkManager instance; 

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

    [Header("TEMP")]
    [SerializeField] private GameObject _characterPrefab = null;
    [SerializeField] private GameObject _localPlayer = null;
    PlayerMovement _localPlayerMovement;

    [SerializeField] private List<Material> _colorMatPlayer = null;

    private List<GameObject> _players = new List<GameObject> { null, null, null, null, null, null };
    private List<bool> _ready = new List<bool> { false, false, false, false, false, false };

    private int _myID;
    private int myTeam;
    private bool isConnect = true;
    private bool isPlayingAgain = false;
    private ProcessorManager _processor;
    private NetworkStoredData networkStored;

    public int NumOfConnectedPlayers { get; private set; } = 1;
    public int GetMyTeam() { return myTeam; }

    //Animation running
    private PlayerAnimation.RunState prevRunState = PlayerAnimation.RunState.stop;

    #region UnityFunctions

    private void OnEnable()
    {
        EventController.AddListener(typeof(ModeChangedMessage), this);
        EventController.AddListener(typeof(NetworkSendBallRespawn), this);
        EventController.AddListener(typeof(ChangeHoleEvent), this);
        EventController.AddListener(typeof(SendShieldMessage), this);
        EventController.AddListener(typeof(SendRunAnimMessage), this);
        EventController.AddListener(typeof(JumpMessage), this);
        EventController.AddListener(typeof(SendClubSwingAnimation), this);
        EventController.AddListener(typeof(PersonalScoreMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ModeChangedMessage), this);
        EventController.RemoveListener(typeof(NetworkSendBallRespawn), this);
        EventController.RemoveListener(typeof(NetworkSendBallRespawn), this);
        EventController.RemoveListener(typeof(ChangeHoleEvent), this);
        EventController.RemoveListener(typeof(SendShieldMessage), this);
        EventController.RemoveListener(typeof(SendRunAnimMessage), this);
        EventController.RemoveListener(typeof(JumpMessage), this);
        EventController.RemoveListener(typeof(SendClubSwingAnimation), this);
        EventController.RemoveListener(typeof(PersonalScoreMessage), this);
    }

    private void Awake()
    {
        _myID = getNetworkID();
        isConnect = _myID >= 0;
        if (!isConnect)
        {
            instance = null;
        }
        else
        {
            instance = this;
            networkStored = NetworkStoredData.instance;
            myTeam = NetworkStoredData.instance ? NetworkStoredData.instance.GetTeam(_myID) : 0;
            _localPlayerMovement = _localPlayer.GetComponent<PlayerMovement>();
            _localPlayer.GetComponent<PlayerScoreTracker>().Init(_myID, true);
            _players.Insert(_myID, _localPlayer);
            _processor = gameObject.transform.GetChild(0).gameObject.GetComponent<ProcessorManager>();
            _processor.StartProcessing();
            SpawnPlayerOnNetwork();               
        }
    }

    private void Start()
    {
        int id = isConnect ? _myID : 0;

        Vector3 location = RespawnManager.instance.GetRespawnLocationAtIndex(id);
        _localPlayer.GetComponent<PlayerHealth>().Respawn(location);

        _localPlayer.transform.GetComponent<PlayerAnimation>().SetMaterial(_colorMatPlayer[myTeam]);
    }

    public void Update()
    {
        if (isConnect)
        {
            updateNetwork();
            SendUpdateForPlayerPosition();
            SendMovementRequest();
        }        
    }

    private void OnApplicationQuit()
    {
        if (!isPlayingAgain)
        {
            print("bye bye network...");
            deleteNetwork();
        }       
    }

    #endregion

    public override void HandleEvent(EventMessage e)
    {
        if (!isConnect) { return; }

        if (e is NetworkSendBallRespawn respawn)
        {
            SendBallMessage(respawn.ball.gameObject, respawn.ball.transform.GetSiblingIndex());
            SendTeamScore(respawn.teamScorer);
        }
        else if (e is ChangeHoleEvent)
        {
            string s = SerializationScript.SerlializeChangeHoleEvent();
            SendData(s, 1, true);
        }
        else if (e is SendShieldMessage shield)
        {
            SendShieldMessage(shield.id);
        }
        else if (e is SendRunAnimMessage runAnim)
        {
            SendRunAnimationState(runAnim.state);
        }
        else if (e is JumpMessage)
        {
            SendJumpMessage();
        }
        else if (e is ModeChangedMessage modeChange)
        {
            SendModeChange(modeChange.modeChangedToGolf);
        }
        else if (e is SendClubSwingAnimation swingAim)
        {
            SendClubSwingAnim(swingAim.activeClub);
        }else if(e is PersonalScoreMessage personalScore)
        {
            SendPersonalScore(personalScore.id, personalScore.type);
        }
    }


    #region SEND DATA
    public void SendPersonalScore(int id, PersonalScoreManager.TYPE type)
    {
        string s = SerializationScript.SerlizalizePersonalScore(id, type);
        SendData(s, 1, true);
    }
    public void SendClubSwingAnim(int clubIndex)
    {
        string s = SerializationScript.SerlizalizeGolfSwingIndex(_myID, clubIndex);
        SendData(s, 1, true);
    }

    public void SendModeChange(bool toGolf)
    {
        string s = SerializationScript.SerlializeModeChange(_myID, toGolf);
        SendData(s, 1, true);
    }

    private void SendJumpMessage()
    {
        string s = SerializationScript.SerlializeJumpMessage(_myID);
        SendData(s, 1, true);
        if (IsHost())
        { 
            //TODO: ADD to jump superlatives
        }
       
    }

    public void SendRunAnimationState(PlayerAnimation.RunState state)
    {
        if(state != prevRunState)
        {
            string s = SerializationScript.SerlializeRunAnimationState(_myID, state);
            SendData(s, 1, true);
            prevRunState = state;
        }       
    }

    public void SendShieldMessage(int id)
    {
        string s = SerializationScript.SerlializeShieldMessage(id);
        SendData(s, 1,true);
        _processor.playerManager.TurnOnShield(s.Split(':'));
    }

    public void SendTeamScore(int team)
    {
        string s = SerializationScript.SerializeTeamScore(team);
        SendData(s, 2, _myID == 0);
        if (IsHost()) { _processor.gameStateManager.UpdateTeamScore(s.Split(':')); }
    }

    public void SendScoreInfo(int id, int current, int? best, int kills, int deaths)
    {
        string s = SerializationScript.SerializeScore(id, current, best, kills, deaths);
        SendData(s, 2, _myID == 0);
    }

    public void SendUpdatedScore(int id)
    {
        PlayerScoreTracker score = _players[id].GetComponent<PlayerScoreTracker>();
        SendScoreInfo(id, score.CurrentStrokes, score.PersonalBest, score.Kills, score.Deaths);
    }

    public void SendUpdatedScore(GameObject obj)
    {
        int id = _players.IndexOf(obj);
        SendUpdatedScore(id);
    }

    public void SendDamageMessage(GameObject obj, int shooterID)
    {
        int id = _players.IndexOf(obj);
        IHaveHealth health = _players[id].GetComponent(typeof(IHaveHealth)) as IHaveHealth;
        float currentHealth = health.GetHealth();
        
        string s = SerializationScript.SerializePlayerHealth(id, currentHealth, shooterID);
        SendData(s, 1, true);
    }

    public void SendGoalUpdate(GameObject ball)
    {
        if (_myID != 0)
            return;

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = RespawnManager.instance.GetRandonBallRespawnLocation();

        int ballID = ball.transform.GetSiblingIndex();
        PlayerScoreTracker scoreTracker = _players[ballID].GetComponent<PlayerScoreTracker>();
        scoreTracker.ConvertPoints();
      
        SendScoreInfo(ballID, scoreTracker.CurrentStrokes, scoreTracker.PersonalBest, scoreTracker.Kills, scoreTracker.Deaths);
    }

    public void SendUpdateForPlayerPosition()
    {
        string s = SerializationScript.SerializeTransform("12", _localPlayer.transform, GameEvent.PLAYER_TRANSFORM);
        SendData(s, 1, true);
    }

    public void SendRequestToHitBall(GameObject ball, int id, Vector3 dir, bool isAbilityBall, bool shouldHome, int targetID)
    {
        if (id < 0)
            id = _processor.ballManager.golfBallManager.GetIdOfAbilityBall(ball.GetComponent<AbilityBall>());

        string s = SerializationScript.SerializeBallMessage(dir, id, _myID, isAbilityBall, shouldHome, targetID);
        if (_myID == 0) { _processor.ballManager.UpdateBalls(s); }
        else { SendData(s, 1, false); }
    }

    public void SendBallMessage(GameObject ball, int id, bool isAbilityBall = false)
    {
        string s = SerializationScript.SerializeTransform("1", ball.transform, GameEvent.BALL_TRANSFORM, isAbilityBall);
        SendData(s, 1, true, id);

        Vector3 move = ball.GetComponent<Rigidbody>().velocity;
        s = SerializationScript.SerializeBallVelocityMessage(move, id, isAbilityBall);
        SendData(s, 1, true, id);
    }

    public void SpawnPlayerOnNetwork()
    {
        string s = SerializationScript.SerializeSpawnMessage(_myID, myTeam);
        SendData(s, 1, true);            
    }

    public void SendMovementRequest()
    {
        Vector3 move = _localPlayerMovement.Movement;
        string s = SerializationScript.SerializePlayerVelocity(_myID, move);
       
        if (_myID == 0)
            _processor.playerManager.UpdatePlayerMovement(s, 0);
       
        SendData(s, 1, true);
    }

    public void SendSwingFuelEvent(int splatterIndex, float duration)
    {
        string s = "Item:SwingFuel:" + splatterIndex + ":" + duration;
        SendData(s, 1, true);
    }

    public void SendPortableSpeakers(Vector3 direction, Vector3 position, float blastDistance, float stunTime, float visualTime)
    {
        string s = "Item:Speakers:" + SerializationScript.Vector3ToString(direction) + ":" + SerializationScript.Vector3ToString(position) + ":" + blastDistance + ":" + stunTime + ":" + visualTime;
       if (_myID == 0)
           _processor.itemManager.ProcessItem(s.Split(':'));
       else
           SendData(s, 1, true);
    }

    public void SendKillFeedMessage(int killer, int victim)
    {
        string s = SerializationScript.SerializeKillFeed(killer, victim);
        SendData(s, 2, true);
        
        if (IsHost()) { _processor.gameStateManager.UpdateKillFeed(s.Split(':')); }
           
    }

    public void SendRespawnUpdate(Vector3 position, int id)
    {
        string s = SerializationScript.SerializeRespawnMessage(position, id);
        SendData(s, 1, true);
    }

    public void SendEndGameRequest()
    {
        if (IsHost())
        {
            string s = SerializationScript.SerializeEndGameMessage();
            SendData(s, 1, true);
            _processor.gameStateManager.CallEndGameEvent();
        }       
    }

    public void SendReadyUp()
    {
        if (!_ready[_myID])
        {
            _processor.gameStateManager.CallReadyEvent(new NetworkReadyUp(_myID));
            _ready[_myID] = true;
           
        
            string s = SerializationScript.SerializeReadyUpMessage(_myID);
            SendData(s, 1, true);
        }
        
        if (IsHost())
        {
            if (_ready.Contains(false)) { print("Waiting for all players to ready up!"); }
            else
            {
                string s = SerializationScript.SerializeRestartGameMessage();
                SendData(s, 1, true);
                
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
            }
        }
        else
        {
            print("Waiting on host!");
        }
    }

    public void SendItemUseRequest(ItemBoxController.Items itemID, List<float> floatInfo, List<int> intInfo, bool sendToAll = true)
    {
        _processor.itemManager.SendItemUseRequest(itemID, floatInfo,intInfo, sendToAll);
    }

    public void SendHealRequest(float healAmount, int id, int boxID = -1)
    {
        string s = SerializationScript.SerializeHealGameMessage(id, healAmount, boxID);
        SendData(s, 1, true);
        if (IsHost()) { _processor.itemManager.HealthPlayer(s.Split(':')); }
    }

    public void SendHealRequest(float healAmount, GameObject obj, int boxID = -1)
    {
        int id = GetIdOfPlayer(obj);
        string s = SerializationScript.SerializeHealGameMessage(id, healAmount, boxID);
        SendData(s, 1, true);
        if (IsHost()) { _processor.itemManager.HealthPlayer(s.Split(':')); }
    }

    public void SendItemBoxUsed(int boxID, GameObject player)
    {
        int id = GetIdOfPlayer(player);
        string s = SerializationScript.SerializeItemBoxPickUp(id, boxID);
        SendData(s, 1, true);
        _processor.itemManager.UpdateItemBox(s.Split(':'));
    }

    public void SendPayRespects()
    {
        string s = SerializationScript.SerializePayRespectMessage(_myID);
        SendData(s, 1, true);
    }

    public void SendAbilityBall(int index, Vector3 position)
    {
        string s = SerializationScript.SerializeAbilitySpawn(index, position);
        SendData(s, 1, true);
        if (IsHost()) { _processor.ballAbilityManager.SpawnBall(s.Split(':')); }
    }

    public void SendWorldVFX(WorldSpaceVFX.WorldVFXType type, Vector3 positio, int idOfTransform = -1, int idOfBall = -1)
    {
        string s = SerializationScript.SerializeWorldVFX(type, positio, idOfTransform, idOfBall);
        SendData(s, 1, true);
    }

    public void SendTriggerdAbilityEvent(int id, GameEvent theEvent, params object[] list)
    {
        string s = SerializationScript.SerializeTriggerdAbilityEvent(id, theEvent, list);
        SendData(s, 1, true);
        _processor.ballAbilityManager.DoAbilityEvent(s.Split(':'),0);
    }

    #endregion

    #region PlayerChanges/INFO

    public void ReadyAPlayer(int id) { _ready[id] = true; }

    public void AddNumberOfConnectedPlayers() { NumOfConnectedPlayers++; }

    public Vector3 GetNetworkPlayerPosition(int id){ return _players[id].transform.position;}

    public GameObject GetBallOfID(int id) { return null; }//return _balls[id]; } //TO DO: FIX THIS ISSUE

    public GameObject GetPlayerOfID(int id) { return _players[id]; }

    public int GetIdOfPlayer(GameObject player) { return _players.IndexOf(player); } 

    public void AddKillToPlayer(int id) { _players[id].GetComponent<PlayerScoreTracker>().ScoreFromKill(); }

    public void AddBallAndPlayerToNetwork(GameObject player, int id) { _players[id] = (player); }

    #endregion

    #region GETTERS
    public int GetId() { return _myID; }
    public bool IsHost() { return _myID == 0; }
    public GameObject GetLocalPlayer() { return _localPlayer; }
    public Material GetMaterialOfPlayer(int id) { return _colorMatPlayer[id]; }
    public List<GameObject> GetAllPlayers() { return _players; }
    #endregion
}

