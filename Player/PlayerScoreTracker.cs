///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerScoreTracker
///   Description:      Tracks the score of the client and calls the relevant
///                     UI scripts to update the visuals to reflect current 
///                     scores.
///   Author:           Parker Staszkiewicz
///   Contributor(s):   Mark Botaish  
///-------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerScoreTracker : EventListener
{
    private ScoreboardController scoreboard;
    private GameplayUIController gameUI;

    bool hasScoreBefore = false;

    public int ID { get; private set; }
    public int CurrentStrokes { get; private set; }
    public int? PersonalBest { get; private set; }
    public int Kills { get; private set; }
    public int Deaths { get; private set; }

    private bool isLocal = true;

    // 1.1 Specific 

    private int golfEfficiencyIndex = 0;
    public int KillingEfficiencyLevel { get; private set; }
    private bool hasGolfingEfficiency = false;
    public int GolfEfficiencyModAmount { get; private set; }

    public void Init(int id, bool tof = false)
    {
        ID = id;
        isLocal = tof;
    }

    public void Start()
    {
        //CanvasController canvController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        //scoreboard = canvController.Scoreboard;
        //gameUI = canvController.GameUI;
        //weaponUI = canvController.WeaponUI;
        KillingEfficiencyLevel = 1;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is GolfStrokeMessage)
        {
            AddStroke();
        }
    }

    public void UpdateEntity(int playerIndex, int strokeVal, int? personalBestVal, int killsVal, int deathsVal, bool isLocal)
    {
        if (isLocal)
        {
            gameUI.UpdatePoints(strokeVal, personalBestVal);
        }          

        scoreboard.UpdateEntity(playerIndex, strokeVal, personalBestVal, killsVal, deathsVal);
    }

    

    public void StartGolfingEfficiency() { hasGolfingEfficiency = true; }
    public void SetGolfEfficiencyAmount(int value) { GolfEfficiencyModAmount = value; }
    public void SetKillingEfficiency(int value) { KillingEfficiencyLevel = value; }

    public void ScoreFromKill()
    {
        Kills += 1;
        CurrentStrokes -= PointsTable.PointsForKill;
        scoreboard.UpdateEntity(ID, CurrentStrokes, PersonalBest, Kills, Deaths);

        if (isLocal) { gameUI.UpdatePoints(CurrentStrokes, PersonalBest); } 
    }

    public void AddStroke()
    {
        if (hasGolfingEfficiency)
        {
            golfEfficiencyIndex = ((golfEfficiencyIndex + 1) % GolfEfficiencyModAmount);
            if (golfEfficiencyIndex == 0)
                CurrentStrokes++;
        }
        else
        {
            CurrentStrokes++;
        }

        scoreboard.UpdateEntity(ID, CurrentStrokes, PersonalBest, Kills, Deaths);
        if (isLocal) { gameUI.UpdatePoints(CurrentStrokes, PersonalBest); }
    }

    public void AddDeath()
    {
        //CurrentStrokes++;
        Deaths++;
        //scoreboard.UpdateEntity(ID, CurrentStrokes, PersonalBest, Kills, Deaths);
        //if (isLocal) { gameUI.UpdatePoints(CurrentStrokes, PersonalBest); }
    }

    public void ConvertPoints()
    {
        if (CurrentStrokes < PersonalBest || !hasScoreBefore)
            PersonalBest = CurrentStrokes;

        CurrentStrokes = 0;
        hasScoreBefore = true;
        scoreboard.UpdateEntity(ID, CurrentStrokes, PersonalBest, Kills, Deaths);
        if (isLocal) { gameUI.UpdatePoints(CurrentStrokes, PersonalBest); SoundManager.PlaySound("Nice Shot"); }
    }
}
