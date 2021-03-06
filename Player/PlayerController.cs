///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerController
///   Description:      Checks for input from the player and manages
///                     logic for the gameplay systems tied to the player.
///                     Updates the CanvasController with client information.
///   Author:           Parker Staszkiewicz
///   Contributor(s):   Mark Botaish
///-------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(PlayerMovement), typeof(GolfClubController), typeof(PlayerHealth))]
public class PlayerController : EventListener
{
    private PlayerMovement playerMove;
    private GolfClubController gcController;
    //private CanvasController canvController;
    private PlayerAnimation playerAnimation;

    private bool acceptInput = false;
    public bool IsDead { get; private set; } = false;
    private bool isGameOver = false;
    private bool isConfused = false;
    private bool lerpToBall = false;
    private Vector3 ballPos;

    private AbilityBall.Abilities[] myAbilities;

    private void Start()
    {
      
        myAbilities = new AbilityBall.Abilities[3];
        int id = NetworkManager.instance ? NetworkManager.instance.GetId() : 0;
        if (NetworkStoredData.instance)
        {           
            myAbilities[0] = (AbilityBall.Abilities)(NetworkStoredData.instance.GetOffenseAbility(id) + 1);
            myAbilities[1] = (AbilityBall.Abilities)(NetworkStoredData.instance.GetDefenseAbility(id) + 3);

        }
        else
        {
            myAbilities[0] = AbilityBall.Abilities.iceBall;
            myAbilities[1] = AbilityBall.Abilities.shieldBall;
        }


        myAbilities[2] = AbilityBall.Abilities.storageBall;

        playerMove = GetComponent<PlayerMovement>();
        gcController = GetComponent<GolfClubController>();
        playerAnimation = GetComponent<PlayerAnimation>();
        //canvController = GameObject.Find("Canvas").GetComponent<CanvasController>();

        EventController.FireEvent(new InitializeAbilityBall(myAbilities[0], id));
        EventController.FireEvent(new InitializeAbilityBall(myAbilities[1], id));
        //Storage ball always needs their event fired last with the current system
        //We may need to refactor this later
        EventController.FireEvent(new InitializeAbilityBall(myAbilities[2], id));

        //
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(SettingsMenuToggleMessage), this);
        EventController.AddListener(typeof(ClientDeathMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);
		EventController.AddListener(typeof(GameStartMessage), this);
		EventController.AddListener(typeof(NearbyGolfBallMessage), this);
		EventController.AddListener(typeof(SwingAnimationMessage), this);
		EventController.AddListener(typeof(SwingClubMessage), this);
		EventController.AddListener(typeof(MakeSureThePlayerCanMoveMessage), this);
		EventController.AddListener(typeof(EndGameMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(SettingsMenuToggleMessage), this);
        EventController.RemoveListener(typeof(ClientDeathMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);
		EventController.RemoveListener(typeof(GameStartMessage), this);
		EventController.RemoveListener(typeof(NearbyGolfBallMessage), this);
		EventController.RemoveListener(typeof(SwingAnimationMessage), this);
		EventController.RemoveListener(typeof(SwingClubMessage), this);
		EventController.RemoveListener(typeof(MakeSureThePlayerCanMoveMessage), this);
		EventController.RemoveListener(typeof(EndGameMessage), this);
	}

    public override void HandleEvent(EventMessage e)
    {
        if (e is ClientDeathMessage)
        {
            ToggleDeath(true);
        }
        else if (e is ClientRespawnMessage)
        {
            ToggleDeath(false);
        }
        else if (e is SettingsMenuToggleMessage toggle)
        {
            ToggleAcceptInput(!toggle.toggledOn);
        }
		else if (e is GameStartMessage)
		{
			ToggleAcceptInput(true);
		} else if (e is NearbyGolfBallMessage golfBallMessage)
        {
            lerpToBall = golfBallMessage.golfBall?.gameObject;
            ballPos = lerpToBall ? golfBallMessage.golfBall.transform.position : Vector3.zero;
        }
        else if(e is SwingAnimationMessage || e is MakeSureThePlayerCanMoveMessage)
        {
            ToggleAcceptInput(true);
        }
        else if (e is SwingClubMessage)
        {
            ToggleAcceptInput(false);
        }
        else if (e is EndGameMessage)
        {
            ToggleAcceptInput(false);
            SetRunAnimation(0, 0);
        }

    }

    private void Update()
    {
        if (acceptInput)
        {
            if (!IsDead && !isGameOver) // Cannot do while dead or game is over
            {
                HandleClubSwitching();
                UseClub();
                HandleMiscInput();
                UseAbilityBall();
                if (lerpToBall)
                    LerpPlayerToBall();
            }

            if (!isGameOver) // Can do while dead, but not when game is over
            {
                ToggleUIElements();
            }
        }

        HandlePlayerMovement();
    }

    private void ToggleDeath(bool isDead)
    {
        IsDead = isDead;
    }

    private void ToggleConfusion()
    {
        isConfused = !isConfused;
    }

    private void SetGameOver()
    {
        isGameOver = true;
        playerMove.MovePlayer(0, 0, false);
    }

    private void ToggleAcceptInput(bool allowInput)
    {
        acceptInput = allowInput;

        if (!acceptInput)
        {
            gcController.SecondaryUseEnd();
            //canvController.ShowScoreboard(false);
        }
    }

    private void ToggleUIElements()
    {
        //if (!canvController.ScoreboardIsActive && KeybindingController.GetInputDown(GameControls.ShowScoreboard))
        //{
        //    canvController.ShowScoreboard(true);
        //}
        //else if (canvController.ScoreboardIsActive && !KeybindingController.GetInput(GameControls.ShowScoreboard))
        //{
        //    canvController.ShowScoreboard(false);
        //}
    }

    private void LerpPlayerToBall()
    {
        Vector3 lerpPos = ballPos + -transform.forward;
        if (Vector3.Distance(transform.position, lerpPos) > 0.5f && horizontalMove == 0 && verticalMove == 0)
            transform.position = Vector3.Lerp(transform.position, lerpPos, Time.deltaTime * 3);
    }
    float horizontalMove;
    float verticalMove;
    private void HandlePlayerMovement()
    {
        horizontalMove = 0;
        verticalMove = 0;
        bool jump = false;

        // If we aren't accepting input / the player gets stunned, we still need to be able to fall from gravity
        // Therefore, we ensure the Move vars stay at 0
        if (acceptInput && !IsDead && !isGameOver)
        {
            if (KeybindingController.GetInput(GameControls.Sprint) && gcController.CurrentMode == GolfClubController.Mode.Running) // Can't sprint while golfing
            {
                playerMove.ShiftSpeed();
            }

            if (!isConfused)
            {
                if (KeybindingController.GetInput(GameControls.MoveForward)) { verticalMove = 1; }
                else if (KeybindingController.GetInput(GameControls.MoveBackward)) { verticalMove = -1; }
                else { playerAnimation.SetRun(PlayerAnimation.RunState.stop); }

                if (KeybindingController.GetInput(GameControls.StrafeRight)) { horizontalMove = 1; }
                else if (KeybindingController.GetInput(GameControls.StrafeLeft)) { horizontalMove = -1; }
            }
            else
            {
                if (KeybindingController.GetInput(GameControls.MoveForward)) { horizontalMove = 1; }
                else if (KeybindingController.GetInput(GameControls.MoveBackward)) { horizontalMove = -1; }

                if (KeybindingController.GetInput(GameControls.StrafeRight)) { verticalMove = -1; }
                else if (KeybindingController.GetInput(GameControls.StrafeLeft)) { verticalMove = 1; }
            }

            SetRunAnimation(horizontalMove, verticalMove);

            jump = KeybindingController.GetInputDown(GameControls.Jump);

            playerMove.RotatePlayer();
        }

        playerMove.MovePlayer(horizontalMove, verticalMove, jump);
    }

    private void SetRunAnimation(float horizontalMove, float verticalMove)
    {
        bool IKFlag = false;
        switch (verticalMove)
        {
            case -1:
                switch (horizontalMove)
                {
                    case -1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.backwardLeft);
                        break;
                    case 0:
                        playerAnimation.SetRun(PlayerAnimation.RunState.backward);
                        break;
                    case 1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.backwardRight);
                        break;
                }
                break;
            case 0:
                switch (horizontalMove)
                {
                    case -1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.left);
                        break;
                    case 0:
                        playerAnimation.SetRun(PlayerAnimation.RunState.stop);
                        IKFlag = true;
                        break;
                    case 1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.right);
                        break;
                }
                break;
            case 1:
                switch (horizontalMove)
                {
                    case -1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.forwardLeft);
                        break;
                    case 0:
                        playerAnimation.SetRun(PlayerAnimation.RunState.forward);
                        break;
                    case 1:
                        playerAnimation.SetRun(PlayerAnimation.RunState.forwardRight);
                        break;
                }
                break;
        }

        if (IKFlag)
            EventController.FireEvent(new StartIKMessage());
        else
            EventController.FireEvent(new StopIKMessage());

    }

    private void UseAbilityBall()
    {
        if(KeybindingController.GetInputDown(GameControls.AbilityBallOne))
        {
            EventController.FireEvent(new AbilityBallPrepareMessage(this, myAbilities[0], 0));
        }
        else if(KeybindingController.GetInputDown(GameControls.AbilityBallTwo))
        {
            EventController.FireEvent(new AbilityBallPrepareMessage(this, myAbilities[1], 1));
        }
        else if (KeybindingController.GetInputDown(GameControls.AbilityBallThree))
        {
            EventController.FireEvent(new AbilityBallPrepareMessage(this, myAbilities[2], 2));
        }
    }

    private void UseClub()
    {
        // PrimaryAction
        if (KeybindingController.GetInputDown(GameControls.PrimaryUse) && playerMove.Movement.magnitude == 0)
        {
            gcController.PrimaryUseStart();
        }
        else if (KeybindingController.GetInput(GameControls.PrimaryUse))
        {
            gcController.PrimaryUseHeld();
        }
        else if (KeybindingController.GetInputUp(GameControls.PrimaryUse))
        {
            gcController.PrimaryUseEnd();
            if(gcController.CurrentMode == GolfClubController.Mode.Golfing){
				playerAnimation.SetClubSwing(gcController.GetActiveClubIndex());
            	EventController.FireEvent(new SendClubSwingAnimation(gcController.GetActiveClubIndex()));
			}
                
        }

        // Secondary Action
        if (KeybindingController.GetInputDown(GameControls.SecondaryUse))
        {
            gcController.SecondaryUseStart();
        }
        else if (KeybindingController.GetInput(GameControls.SecondaryUse))
        {
            gcController.SecondaryUseHeld();
        }
        else if (KeybindingController.GetInputUp(GameControls.SecondaryUse))
        {
            gcController.SecondaryUseEnd();
        }
    }

    private void HandleClubSwitching()
    {
        if (KeybindingController.GetInputDown(GameControls.NextClub))
        {
            gcController.ScrollClub(1);
        }
        else if (KeybindingController.GetInputDown(GameControls.PreviousClub))
        {
            gcController.ScrollClub(-1);
        }
        else if (KeybindingController.GetInputDown(GameControls.SwitchToClubOne))
        {
            gcController.ChooseClub(0);
        }
        else if (KeybindingController.GetInputDown(GameControls.SwitchToClubTwo))
        {
            gcController.ChooseClub(1);
        }
        else if (KeybindingController.GetInputDown(GameControls.SwitchToClubThree))
        {
            gcController.ChooseClub(2);
        }
    }

    private void HandleMiscInput()
    {
        if (KeybindingController.GetInputDown(GameControls.PayRespects))
        {
            if (NetworkManager.instance) { NetworkManager.instance.SendPayRespects(); }                
        }
    }
}
