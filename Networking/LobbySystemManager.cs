using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class LobbySystemManager : EventListener
{
    #region PLUGIN
    [DllImport("egp-net-plugin-Unity")] static extern void updateNetwork();                                                                // Call the update loop in the plugin to process packets 
    [DllImport("egp-net-plugin-Unity")] static extern void deleteNetwork();                                                                // Disconnect/close any connections 
    [DllImport("egp-net-plugin-Unity")] static extern void DisconnectFromMaster();                                                                // Disconnect/close any connections 
    [DllImport("egp-net-plugin-Unity")] static extern void startNetwork(string name);                                                                 // Become a host and start the network 
    [DllImport("egp-net-plugin-Unity")] static extern int getNetworkID();                                                                  // Get your network ID
    [DllImport("egp-net-plugin-Unity")] static extern void connectToNetwork(int index);                                                    // Connect to a specific IP address 
    [DllImport("egp-net-plugin-Unity")] static extern void ConnectToMasterServer(string ip);                                               // Connect to a specific IP address 
    [DllImport("egp-net-plugin-Unity")] static extern bool hasPriorityMessage();                                                           // Check to see if a priority message has been recieved 
    [DllImport("egp-net-plugin-Unity")] static extern bool hasNormalMessage();                                                             // Check to see if a normal message has been recieved
    [DllImport("egp-net-plugin-Unity")] static extern bool checkLobbyStateChange();                                                        // Check to see if the lobby state has changed
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr getPriorityMessage(ref int id, ref int delay);                                // Get the information of a priority message
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr getNormalMessage(ref int id);                                                 // Get the information of a normal message
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr GetLobbyIPAtIndex(int index);                                                 // Get the lobby ip at index 
    [DllImport("egp-net-plugin-Unity")] static extern IntPtr GetLobbyNameAtIndex(int index);                                                 // Get the lobby ip at index 
    [DllImport("egp-net-plugin-Unity")] static extern int SendData(string data, int PriorityNum, bool sendAll, int id = -1);               // Send packet. Priority number is either 1 or 2 corresponding to a priority message and normal message respectively
    #endregion

    [Tooltip("The scene index for the main game scene")] public int sceneIndex = 1;
    [Header("Local Testing Only!")] public bool isMasterServerLocal = false;

    private string url = "https://docs.google.com/spreadsheets/d/1x89mjAYoL0AXbRpiJczTHqq1g7yHQczOvRrPK4_9P9U/edit#gid=0&range=A1";

    int id = -1;
    bool isConnectToMaster = false;
    NetworkStoredData data;

    private void OnEnable()
    {
        EventController.AddListener(typeof(JoinLobby), this);
        EventController.AddListener(typeof(SendAbilityBallUpdate), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(JoinLobby), this);
        EventController.RemoveListener(typeof(SendAbilityBallUpdate), this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("Destory Existing Network");

        if (isMasterServerLocal) { Debug.LogError("CONNECTING TO LOCAL MASTER SERVER (TESTING ONLY)"); }

        deleteNetwork();
        StartCoroutine(ConnectToMaster());
        data = NetworkStoredData.instance;

    }

    IEnumerator ConnectToMaster()
    {      
        string finalIp = "127.0.0.1";

        if (!isMasterServerLocal)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            string[] results;
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Cound not find master IP");
            }
            else
            {
                results = www.downloadHandler.text.Split('\n');
                finalIp = (Regex.Match(results[2], ".+?(?=\")").ToString());
            }
        }
       
        ConnectToMasterServer(finalIp);
        while (!checkLobbyStateChange())
        {            
            updateNetwork();
            yield return null;
        }
        isConnectToMaster = true;
        EventController.FireEvent(new CloseWarningMessage());
        EventController.FireEvent(new EnableCurrentPanel());
    }

    IEnumerator ConnectToServer(int index)
    {
        EventController.FireEvent(new OpenWarningMessage("Connecting to Lobby"));
        connectToNetwork(index);
        while (getNetworkID() <= 0)
        {
            updateNetwork();
           
        }
        id = getNetworkID();
        EventController.FireEvent(new CloseWarningMessage());
        EventController.FireEvent(new IDMessage(id));
        yield return null;
        SendData(GameEvent.TEAM_REQUEST.ToString(), 1, false);        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnectToMaster)
        {
            return;
        }
        updateNetwork();
        CheckLobbies();
        HandleMessages();

        if (Input.GetKeyDown(KeyCode.Space) && id == 0) // If you are the host and you press space, then start the game
        {
            if (SendData("LOAD", 1, true) < 0) { Debug.LogError("PACKET FAILED TO SEND"); }
            DisconnectFromMaster();
            SceneManager.LoadScene(sceneIndex);         // Load the start scene 
        }
    }

    /// <summary>
    /// Processing priority messages. The only message to be sent should be a message to start the game 
    /// </summary>
    void HandleMessages()
    {
        int newId = -1;
        int delay = -1;

        while (hasPriorityMessage())
        {
            string msg = Marshal.PtrToStringAnsi(getPriorityMessage(ref newId, ref delay));
            string[] splitCode = msg.Split(':');
            if (splitCode[0] == "LOAD") { SceneManager.LoadScene(sceneIndex); }
            else if (splitCode[0] == GameEvent.TEAM_ASSIGNMENT.ToString()) { UpdateTeams(splitCode); }
            else if (splitCode[0] == GameEvent.TEAM_REQUEST.ToString()) { HandleRequestForTeam(newId); }
            else if (splitCode[0] == GameEvent.ABILITY_CHANGE.ToString()) { HandleAbilityChange(splitCode); }
            else { Debug.LogError("Message < " + msg + " > was not processed"); }
        }      
    }
    public void CheckLobbies()
    {
        int index = 0;
        if (checkLobbyStateChange())
        {
            EventController.FireEvent(new LobbyStateChange());
        }

        while (checkLobbyStateChange())
        {          
            string ip = Marshal.PtrToStringAnsi(GetLobbyIPAtIndex(index));
            string name  = Marshal.PtrToStringAnsi(GetLobbyNameAtIndex(index));
           
            if (ip != "")
            {              
                EventController.FireEvent(new LobbyOpened(index, name));
            }
            index++;
        }
    }


    public void HostPlease()
    {
        EventController.FireEvent(new NameMessage(StartHosting, "Enter a lobby name"));
    }

    public void StartHosting(string name, GameObject obj)
    {
        obj.SetActive(false);
        startNetwork(name);       
        id = 0;
        EventController.FireEvent(new IDMessage(id));
        EventController.FireEvent(new StartLobby());
        HandleRequestForTeam(0);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is JoinLobby lobby)
        {
            StartCoroutine(ConnectToServer(lobby.index));
        }
        if(e is SendAbilityBallUpdate abilityBall)
        {
            UpdateAbilityBalls(abilityBall.flag, abilityBall.id, abilityBall.value);
        }
    }

    public void UpdateAbilityBalls(int flag, int id, int value)
    {
        data.SetAbilityBall(id, value, flag);
        string s = SerializationScript.SerializeAbilityChangeEvent(id, flag, value);
        SendData(s, 1, true);
    }

    public void HandleAbilityChange(string[] splitcode)
    {
        int id = int.Parse(splitcode[1]);
        int flag = int.Parse(splitcode[2]);
        int value = int.Parse(splitcode[3]);
        NetworkStoredData.instance.SetAbilityBall(id, value, flag);
    }

    private void HandleRequestForTeam(int id)
    {
        data.AddToTeam(id, id % 2);
        string s = SerializationScript.SerializeTeamAssignment(NetworkStoredData.instance.GetTeam());
        SendData(s, 1, true);
        EventController.FireEvent(new ConnectedUsersUpdate(data.GetTeam()));
    }

    private void UpdateTeams(string[] splitCode)
    {
        int index = 1;
        while (index != splitCode.Length)
        {
            int id = int.Parse(splitCode[index]);
            int team = int.Parse(splitCode[index + 1]);
            data.AddToTeam(id, team);
            index += 2;
        }
        EventController.FireEvent(new ConnectedUsersUpdate(data.GetTeam()));
    }

    private void OnApplicationQuit()
    {
        Debug.LogError("Destory Existing Network");
        deleteNetwork();
    }

   
}
