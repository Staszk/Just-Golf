///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            LobbyManager
///   Description:      Storing information of the user during the menu scene.
///                     This can include: team, character model, ball ability,etc
///   Author:           Mark Botaish
///------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStoredData : MonoBehaviour
{

    public static NetworkStoredData instance = null;

    public List<int> teamOfPlayers = new List<int>();
    private List<int> abilityBallOffense = new List<int>();
    private List<int> abilityBallDefense = new List<int>();
    

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }       
    }


    public void AddToTeam(int id, int team)
    {        
        while (id >= teamOfPlayers.Count)
        {
            teamOfPlayers.Add(-1);
            abilityBallOffense.Add(0);
            abilityBallDefense.Add(0);
        }

        teamOfPlayers[id] = team;
    }

    public void SetAbilityBall(int id, int value, int flag)
    {

        while (id >= abilityBallOffense.Count)
        {
            abilityBallOffense.Add(0);
            abilityBallDefense.Add(0);
        }

        if (flag == 0)
        {
            abilityBallOffense[id] = value;
        }
        else
        {
            abilityBallDefense[id] = value;
        }
    }

    public void SwitchToTeam(int id, int newTeam)
    {
        if (id >= teamOfPlayers.Count) { Debug.LogError("ID:" + id + " needs a team first."); return; }
        teamOfPlayers[id] = newTeam;
    }

    public int GetTeam(int id) {return teamOfPlayers[id]; }
    public int GetOffenseAbility(int id) { return abilityBallOffense[id]; }
    public int GetDefenseAbility(int id) { return abilityBallDefense[id]; }
    public List<int> GetTeam() { return teamOfPlayers; }   
    
    public int GetIDofTeammate(int id)
    {
        List<int> temp = GetTeam();
        temp.Remove(id);
        return temp[0];
    }
}
