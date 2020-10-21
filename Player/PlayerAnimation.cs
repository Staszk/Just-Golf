///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerAnimation
///   Description:      Listens for gameplay events that dictate the animation
///                     blending for the player model.
///   Author:           Parker Staszkiewicz
///   Contributor(s):   Mark Botaish  
///-------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class PlayerAnimation : EventListener
{
    public enum RunState
    {
        forward,
        forwardRight,
        right,
        backwardRight,
        backward,
        backwardLeft,
        left,
        forwardLeft,
        stop = -1
    }

    [SerializeField] private SkinnedMeshRenderer characterMesh = null;

    public Animator animator;

    private GameObject currentPose;

    private void OnEnable()
    {
        EventController.AddListener(typeof(ModeChangedMessage), this);
        EventController.AddListener(typeof(ClientDeathMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);
        EventController.AddListener(typeof(JumpMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ModeChangedMessage), this);
        EventController.RemoveListener(typeof(ClientDeathMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);
        EventController.RemoveListener(typeof(JumpMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ModeChangedMessage modeChange)
        {
            ChangeMode(modeChange);
        }
        else if (e is JumpMessage)
        {
            SetJump();
        }
    }

    private void ChangeMode(ModeChangedMessage eventMessage)
    {
        if (eventMessage.modeChangedToGolf)
        {
            animator.SetInteger("SwingClub", -1);
        }
        else
        {
            animator.SetInteger("SwingClub", -2);
        }
    }

    public void SetJump()
    {
        animator.SetBool("Jump", true);
        StartCoroutine(EndAnimation("Jump"));
    }

    public void SetRun(RunState state)
    {
        animator.SetInteger("Run", (int)state);
        EventController.FireEvent(new SendRunAnimMessage(state));
    }

    public void SetClubSwing(int clubIndex)
    {
        animator.SetInteger("SwingClub", clubIndex);
        StartCoroutine(EndAnimation("SwingClub", -2));
    }

    //For integers
    IEnumerator EndAnimation(string name, int endIndex)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetInteger(name, endIndex);
    }

    //For bools
    IEnumerator EndAnimation(string name)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(name, false);
    }

    public int GetSwingAnimationInt()
    {
        return animator.GetInteger("SwingClub");
    }

    public void SetMaterial(Material playerMat)
    {
        characterMesh.material = playerMat;
    }

}
