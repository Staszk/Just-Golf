///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            GolfClubController
///   Description:      Receives input from PlayerController to utilize the
///                     two modes of a GolfClub class. Uses delegate functions
///                     that change based on mode, so PlayerController does not
///                     need to condition check.
///   Author:           Parker Staszkiewicz
///-------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClubController : EventListener
{
    public enum Mode { Null, Golfing, Running };

    [SerializeField] private GolfClub[] golfClubs = null;
    [SerializeField] private GameObject[] clubModels = null;
    private int clubIndex = 0;

    private PlayerGolf playerGolf;
    private bool nearBall = false;

    #region Delegates
    private delegate void State();
    private State EnterState;
    private State UpdateState;
    private State EndState;

    public delegate void Use();
    public Use PrimaryUseStart;
    public Use PrimaryUseHeld;
    public Use PrimaryUseEnd;
    public Use SecondaryUseStart;
    public Use SecondaryUseHeld;
    public Use SecondaryUseEnd;
    #endregion

    public Mode CurrentMode { get; private set; }

    private void Start()
    {
        int id = NetworkManager.instance == null ? 0 : NetworkManager.instance.GetId();
        // Set up clubs
        for (int i = 0; i < golfClubs.Length; i++)
        {
            golfClubs[i].Initialize(id);
        }

        playerGolf = GetComponent<PlayerGolf>();

        CurrentMode = Mode.Null;
        ChangeMode();
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(NearbyGolfBallMessage), this);
        EventController.AddListener(typeof(AMPBallMessage), this);
        EventController.AddListener(typeof(EndAMPBallMessage), this);
        EventController.AddListener(typeof(SwingAnimationMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(NearbyGolfBallMessage), this);
        EventController.RemoveListener(typeof(AMPBallMessage), this);
        EventController.RemoveListener(typeof(EndAMPBallMessage), this);
        EventController.RemoveListener(typeof(SwingAnimationMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is NearbyGolfBallMessage golfBallMessage)
        {
            if ((golfBallMessage.golfBall && CurrentMode != Mode.Golfing) || !golfBallMessage.golfBall && CurrentMode == Mode.Golfing)
            {
                ChangeMode();
            }
        }
        else if (e is AMPBallMessage amp)
        {
            foreach(GolfClub g in golfClubs)
            {
                g.EditMaxDistance(amp.playerID, amp.rangeMultiplier);
            }
        }
        else if (e is EndAMPBallMessage enpAmp)
        {
            foreach (GolfClub g in golfClubs)
            {
                g.ChangeMaxDistanceBackToOriginal(enpAmp.playerID);
            }
        }
        else if (e is SwingAnimationMessage)
        {
            golfClubs[clubIndex].Swing();
        }
    }

    public GolfClub GetActiveClub()
    {
        return golfClubs[clubIndex];
    }

    public int GetActiveClubIndex()
    {
        return clubIndex;
    }

    public void ScrollClub(int dir)
    {
        int nextIndex = clubIndex + dir;

        if (nextIndex < 0)
        {
            nextIndex = golfClubs.Length - 1;
        }
        else if (nextIndex >= golfClubs.Length)
        {
            nextIndex = 0;
        }

        ChooseClub(nextIndex);
    }

    public void ChooseClub (int index)
    {
        //Debug.Log(clubIndex);

        if (index != clubIndex)
        {
            if (golfClubs[clubIndex].IsPrepped)
                golfClubs[clubIndex].EndShot();

            clubModels[clubIndex].SetActive(false);
            clubIndex = index;
            clubModels[clubIndex].SetActive(true);


            // SEND MESSAGE OF CLUB CHANGE
            EventController.FireEvent(new ClubChangedMessage(clubIndex, golfClubs[clubIndex].ClubStats));
        }
    }

    private void ChangeMode()
    {
        EndState?.Invoke();

        bool isGolfing = false;

        if (CurrentMode == Mode.Golfing || CurrentMode == Mode.Null)
        {
            CurrentMode = Mode.Running;

            EnterState = EnterRunState; UpdateState = UpdateRunState; EndState = EndRunState;
            PrimaryUseStart = RunPrimaryUseStart; PrimaryUseHeld = RunPrimaryUseHeld; PrimaryUseEnd = RunPrimaryUseEnd;
            SecondaryUseStart = RunSecondaryUseStart; SecondaryUseHeld = RunSecondaryUseHeld; SecondaryUseEnd = RunSecondaryUseEnd;
        }
        else if (CurrentMode == Mode.Running) 
        {
            CurrentMode = Mode.Golfing;
            isGolfing = true;

            EnterState = EnterClubState; UpdateState = UpdateClubState; EndState = EndClubState;
            PrimaryUseStart = ClubPrimaryUseStart; PrimaryUseHeld = ClubPrimaryUseHeld; PrimaryUseEnd = ClubPrimaryUseEnd;
            SecondaryUseStart = ClubSecondaryUseStart; SecondaryUseHeld = ClubSecondaryUseHeld;  SecondaryUseEnd = ClubSecondaryUseEnd;
        }

        playerGolf.SetGolfMode(isGolfing);

        EventController.FireEvent(new ModeChangedMessage(isGolfing));

        EnterState();
    }

    #region Club States

    private void EnterClubState()
    {
        EventController.FireEvent(new ClubPowerChangedMessage(golfClubs[clubIndex].ClubStats.MinDistance, true));
        EventController.FireEvent(new ClubChangedMessage(clubIndex, golfClubs[clubIndex].ClubStats));
    }

    private void UpdateClubState()
    {
        
    }

    private void EndClubState()
    {
        golfClubs[clubIndex].EndShot();
    }

    public void ClubPrimaryUseStart()
    {
        golfClubs[clubIndex].PrepareSwing();
    }

    public void ClubPrimaryUseHeld()
    {
        // Debug.Log("Club Held Left Click");
        float power = golfClubs[clubIndex].ChargePower();

        EventController.FireEvent(new ClubPowerChangedMessage(power));
    }

    private void ClubPrimaryUseEnd()
    {
        golfClubs[clubIndex].GolfSwing(playerGolf._GolfBall);
        if(playerGolf._GolfBall)
            EventController.FireEvent(new SwingClubMessage());
        EventController.FireEvent(new MakeSureThePlayerCanMoveMessage());

    }

    private void ClubSecondaryUseStart()
    {
        golfClubs[clubIndex].EndShot();
    }

    public void ClubSecondaryUseHeld()
    {
       // Debug.Log("Club Held Right Click");
    }

    private void ClubSecondaryUseEnd()
    {
        //Debug.Log("Club End Right Click");
    }

    #endregion

    #region Run States

    private void EnterRunState()
    {

    }

    private void UpdateRunState()
    {

    }

    private void EndRunState()
    {
        PrimaryUseEnd();
        SecondaryUseEnd();
    }

    public void RunPrimaryUseStart()
    {

    }

    public void RunPrimaryUseHeld()
    {

    }

    public void RunPrimaryUseEnd()
    {

    }

    public void RunSecondaryUseStart()
    {

    }

    public void RunSecondaryUseHeld()
    {

    }

    public void RunSecondaryUseEnd()
    {

    }

    #endregion
}
