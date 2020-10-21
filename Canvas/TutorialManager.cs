using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    //Each step of the tutorial is a function
    //They are called in order as they are completed.

    //Required panels
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private GameObject objectivesPanel;
    [SerializeField]
    private TMPro.TextMeshProUGUI instructions;
    [SerializeField]
    private TMPro.TextMeshProUGUI objectives;
    [SerializeField]
    private GameObject dummyHolder;

    //Int to keep track of which step the player is in
    private int step = -1;

    private bool moved = false;

    private PlayerTutorialChecker tutorialChecker;

    #region Singleton
    public static TutorialManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        dummyHolder.SetActive(false);
        Intro();
    }

    private void Update()
    {
        //Check for completion of steps
        CheckStepCompletion();

        //Check if user overrides tutorial
        OverrideTutorial();
    }

    #region Text Updates

    void Intro()
    {
        instructions.text = "Hey there ace, Welcome to Just Golf! Before you tee off, let’s run through the basics here.";
        objectives.text = "";
        StartCoroutine(IntroTransition());
    }

    void DoMovement()
    {
        instructions.text = "Use the WASD keys to move around the course. You can also jump with the spacebar";
        objectives.text = "Move";
    }

    void HitPutter()
    {
        instructions.text = "Now let’s get to the sport itself shall we? Use left-click to charge up a shot when you are next to a golf ball. You can cancel with right-click.";
        objectives.text = "Hit the ball";
    }

    void HitRemainingClubs()
    {
        instructions.text = "You need the right club for each situation. Use the 1,2, and 3 keys to alternate between your putter, wedge, and driver. You can also cycle through your clubs using the mouse wheel";
        objectives.text = "Use all three clubs";
    }

    void ColoredBall()
    {
        instructions.text = "The goal here at Just Golf is quite simple really. Work with your teammate, score golf balls into halls, and use your ability balls to hamper the progress of your opponents. The team with the greatest amount of points at the end of the set time is the winner.";
        objectives.text = "Score a colored ball into a hole";
    }

    void AbilityBalls()
    {
        instructions.text = "Now here is our bit of competitive edge, abilities ball. Use the Q, E, and F keys to drop down one of our patented abilities balls. These balls do not grant points if you putt them into the holes on the course.";
        objectives.text = "";
        dummyHolder.SetActive(true);
        StartCoroutine(ABTransition());
    }

    void StorageBall()
    {
        instructions.text = "The F key spawns our basic storage ball, which will be your main tool for combating with other players. You can only have five of these active at once. They also home in on enemies if you are using your Driver!";
        objectives.text = "Use Stroage Ball";
    }

    void OffenseBall()
    {
        instructions.text = "The E key spawns an offensive ball that can disrupt your enemies!";
        objectives.text = "Use Offensive Ball";
    }

    void SupportBall()
    {
        instructions.text = "The Q key spawns a support ball that you can use to assist your teammate!";
        objectives.text = "Use Support Ball";
    }

    void EndTutorial()
    {
        instructions.text = "Well, that’s everything. Hope you and your friends enjoy your time here at Just Golf. See you on the green!";
        objectives.text = "Have Fun!";
        StartCoroutine(EndTutorialSequence());
    }
    #endregion

    public void OverrideTutorial()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            BeginGame();
        }
    }

    public void StepDone()
    {
        step++;

        //Run the correct text field
        switch (step)
        {
            case 1:
                HitPutter();
                break;
            case 2:
                HitRemainingClubs();
                break;
            case 3:
                AbilityBalls();
                break;
            case 4:
                StorageBall();
                break;
            case 5:
                OffenseBall();
                break;
            case 6:
                SupportBall();
                break;
            case 7:
                ColoredBall();
                break;
            case 8:
                EndTutorial();
                break;
            default:
                break;
        }
    }

    void CheckStepCompletion()
    {
            switch (step)
            {
            //Movement step
            case 0:
                if (!moved)
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                    {
                        StartCoroutine(Moved());
                        moved = true;
                    }
                break;
            //Hitting ball first time
            case 1:
                tutorialChecker.CheckFirstHit();
                break;
            //Hitting remaining clubs
            case 2:
                tutorialChecker.CheckRemainingClubs();
                break;
            //Storage Ball
            case 4:
                tutorialChecker.CheckStorageBallSpawn();
                break;
            //Offense Ball
            case 5:
                tutorialChecker.CheckOffenseBallSpawn();
                break;
            //Support Ball
            case 6:
                tutorialChecker.CheckSupportBallSpawn();
                break;
            //Scoring objective ball
            case 7:
                tutorialChecker.CheckBallScored();
                break;
            default:
                break;
            }

    }

    private IEnumerator IntroTransition()
    {
        yield return new WaitForSeconds(5f);
        DoMovement();
        step = 0;
    }

    private IEnumerator ABTransition()
    {
        yield return new WaitForSeconds(5f);
        StepDone();

    }

    private IEnumerator EndTutorialSequence()
    {
        yield return new WaitForSeconds(5f);
        BeginGame();
    }

    private IEnumerator Moved()
    {
        yield return new WaitForSeconds(2f);
        StepDone();
    }

    private void BeginGame()
    {
        SceneManager.LoadScene("Underwater");
    }

    public void FoundTutorialChecker(PlayerTutorialChecker tc)
    {
        tutorialChecker = tc;
    }
}
