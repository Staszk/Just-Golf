using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[assembly: InternalsVisibleTo("NetworkManager")]
public class ProcessorManager : MonoBehaviour
{
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

    [SerializeField] internal NetworkItemManager itemManager = null;
    [SerializeField] internal NetworkRespawnManager respawnManager = null;
    [SerializeField] internal NetworkGameStateManager gameStateManager = null;
    [SerializeField] internal NetworkPlayerManager playerManager = null;
    [SerializeField] internal NetworkBallManager ballManager = null;
    [SerializeField] internal NetworkBallAbilityManager ballAbilityManager = null;
    [SerializeField] internal NetworkPlayerAnimationManager animManager = null;

    private bool startProcessing = false;
    internal void StartProcessing() { startProcessing = true; }

    private int _myID;
    private NetworkManager network = null;

    private void Start()
    {
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();
    }

    // Update is called once per frame
    void Update()
    {
        if (startProcessing)
        {
            CheckMessages();
            CheckNormalMessages();
        }       
    }

    /// <summary> Loops through all the priority messages and sorts the data accordingly. </summary>
    private void CheckMessages()
    {
        while (hasPriorityMessage())
        {
            int newId = -1;
            int delay = -1;
            string msg = Marshal.PtrToStringAnsi(getPriorityMessage(ref newId, ref delay));
            string[] splitCode = msg.Split(':');
            string itemNum = "";
            delay = Mathf.Abs(delay);

            if (splitCode[0] == GameEvent.PLAYER_TRANSFORM.ToString())
            {
                Matrix4x4 mat = SerializationScript.DeserializeTransform(msg, ref itemNum);
                playerManager.UpdateTransform(network.GetAllPlayers()[newId], mat, itemNum, true);
            }
            else if (splitCode[0] == GameEvent.BALL_TRANSFORM.ToString())
            {
                Matrix4x4 mat = SerializationScript.DeserializeTransform(msg, ref itemNum);
                GameObject ball = bool.Parse(splitCode[splitCode.Length - 1]) ? ballManager.GetAbilityBallOfID(newId) : ballManager.GetBallOfID(newId);
                playerManager.UpdateTransform(ball, mat, itemNum, true);
            }

            else if (splitCode[0] == GameEvent.SPAWN.ToString()) { gameStateManager.SpawnPlayerOnNetwork(msg); }
            else if (splitCode[0] == GameEvent.MOVE.ToString()) { playerManager.UpdatePlayerMovement(msg, delay); }
            else if (splitCode[0] == GameEvent.BALL_MOVEMENT.ToString()) { ballManager.UpdateBallMovement(msg, delay); }
            else if (splitCode[0] == GameEvent.ITEM.ToString()) { itemManager.ProcessItem(splitCode, delay); }
            else if (splitCode[0] == GameEvent.HEALTH.ToString()) { playerManager.UpdateHealth(splitCode); }
            else if (splitCode[0] == GameEvent.POSE_CHANGE.ToString()) { playerManager.UpdatePoses(splitCode); }
            else if (splitCode[0] == GameEvent.RESPAWN.ToString()) { respawnManager.UpdateRespawn(splitCode); }
            else if (splitCode[0] == GameEvent.ENDGAME.ToString()) { gameStateManager.UpdateEndGame(); }
            else if (splitCode[0] == GameEvent.READYUP.ToString()) {  gameStateManager.UpdateReadyPlayers(splitCode); }
            else if (splitCode[0] == GameEvent.RESTART_GAME.ToString()) { gameStateManager.UpdateRestartGame(); }
            else if (splitCode[0] == GameEvent.HEAL.ToString()) { itemManager.HealthPlayer(splitCode); }
            else if (splitCode[0] == GameEvent.ITEMBOX.ToString()) { itemManager.UpdateItemBox(splitCode); }
            else if (splitCode[0] == GameEvent.BALL_SHOT.ToString()) { ballManager.UpdateBalls(msg); }
            else if (splitCode[0] == GameEvent.RESPECT.ToString()) { gameStateManager.UpdateRespects(splitCode); }
            else if (splitCode[0] == GameEvent.ABILITY_SPAWN.ToString()) { ballAbilityManager.SpawnBall(splitCode); }
            else if (splitCode[0] == GameEvent.WOLRD_VFX.ToString()) { ballAbilityManager.AbilityBallVFX(splitCode); }
            else if (splitCode[0] == GameEvent.ABILITY_EVENT.ToString()) { ballAbilityManager.DoAbilityEvent(splitCode, delay); }
            else if (splitCode[0] == GameEvent.CHANGE_HOLE.ToString()) { gameStateManager.ChangeHole(); }
            else if (splitCode[0] == GameEvent.SHIELD.ToString()) { playerManager.TurnOnShield(splitCode); }
            else if (splitCode[0] == GameEvent.ANIMATION.ToString()) { animManager.ProcessAnimation(splitCode, delay); }
            else if (splitCode[0] == GameEvent.PERSONAL_SCORE.ToString()) { gameStateManager.UpdatePersonalScore(splitCode); }
            else { Debug.LogError("Message < " + msg + " > was not processed"); }
        }
    }

    /// <summary> Loops through all of the normal messages and sorts accordingly. </summary>
    private void CheckNormalMessages()
    {
        while (hasNormalMessage())
        {
            int newId = -1;
            string msg = Marshal.PtrToStringAnsi(getNormalMessage(ref newId));
            string[] splitCode = msg.Split(':');
            
            if (splitCode[0] == GameEvent.SCORE.ToString()) { gameStateManager.UpdateScore(msg); }
            else if (splitCode[0] == GameEvent.KILLFEED.ToString()) { gameStateManager.UpdateKillFeed(splitCode); }
            else if (splitCode[0] == GameEvent.TEAM_SCORE.ToString()) { gameStateManager.UpdateTeamScore(splitCode); }
            else { Debug.LogError("Message < " + msg + " > was not processed"); }
        }
    }

   
}
