using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script checks for completion of specific tasks for the tutorial manager
public class PlayerTutorialChecker : MonoBehaviour
{
    TutorialManager tM;
    GameObject player;

    #region Singleton
    public static PlayerTutorialChecker instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    //Variables needed for step completion
    private bool putterHit = false;
    private bool wedgeHit = false;
    private bool driverHit = false;
    private bool scored = false;

    // Start is called before the first frame update
    void Start()
    {
        tM = TutorialManager.instance;
        player = transform.parent.gameObject;

        if (tM != null)
        {
            tM.FoundTutorialChecker(this);
        }
    }

    public void CheckFirstHit()
    {
        if (putterHit || wedgeHit || driverHit)
        {
            tM.StepDone();
        }
    }

    public void CheckRemainingClubs()
    {
        if (putterHit && wedgeHit && driverHit)
        {
            tM.StepDone();
        }
    }

    public void ClubWasHit(string clubName)
    {
        if (clubName == "Driver")
        {
            driverHit = true;
        }

        if (clubName == "Wedge")
        {
            wedgeHit = true;
        }

        if (clubName == "Putter")
        {
            putterHit = true;
        }
    }

    public void CheckStorageBallSpawn()
    {
        if (KeybindingController.GetInputDown(GameControls.AbilityBallThree))
        {
            tM.StepDone();
        }
    }
    public void CheckSupportBallSpawn()
    {
        if (KeybindingController.GetInputDown(GameControls.AbilityBallTwo))
        {
            tM.StepDone();
        }
    }

    public void CheckOffenseBallSpawn()
    {
        if (KeybindingController.GetInputDown(GameControls.AbilityBallOne))
        {
            tM.StepDone();
        }
    }

    public void BallScored()
    {
        scored = true;
    }

    public void CheckBallScored()
    {
        if (scored)
        {
            tM.StepDone();
        }
    }

}
