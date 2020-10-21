///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            EnemyAnimation
///   Description:      Updates the poses for the enemies, based on 
///                     the network information
///   Author:           Mark Botaish
///-------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : EventListener
{
    [SerializeField] private SkinnedMeshRenderer golfPoseMesh = null;

    public Animator animator;
    private GameObject currentPose;
    private int id = -1;

    public void Start()
    {
        id = int.Parse(gameObject.name);
        EventController.AddListener(typeof(EnemyRunAnimation), this);
        EventController.AddListener(typeof(EnemyJump), this);
        EventController.AddListener(typeof(EnemyModeChange), this);
        EventController.AddListener(typeof(EnemyClubSwingAnim), this);
    }

    public void OnDestroy()
    {
        EventController.RemoveListener(typeof(EnemyRunAnimation), this);
        EventController.RemoveListener(typeof(EnemyJump), this);
        EventController.RemoveListener(typeof(EnemyModeChange), this);
        EventController.RemoveListener(typeof(EnemyClubSwingAnim), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is EnemyRunAnimation runAnim)
        {
            if (runAnim.id == id) { SetRun(runAnim.state); }
        }else if (e is EnemyJump jumpAnim)
        {
            if (jumpAnim.id == id) { SetJump(jumpAnim.delay); }
        }else if(e is EnemyModeChange modeChange)
        {
            if (modeChange.id == id) { ChangeMode(modeChange.isGolfMode); }
        }
        else if (e is EnemyClubSwingAnim clubSwing)
        {
            if (clubSwing.id == id) { SetClubSwing(clubSwing.activeClub, clubSwing.delay); }
        }
    }

    private void ChangeMode(bool isGolfMode)
    {
        if (isGolfMode)
        {
            animator.SetInteger("SwingClub", -1);
        }
        else
        {
            animator.SetInteger("SwingClub", -2);
        }
    }

    public void SetRun(int state)
    {
        animator.SetInteger("Run", state);
    }

    public void SetMaterial(Material playerMat)
    {
        golfPoseMesh.material = playerMat;
    }

    public void ToggleGameObject()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SetJump(float delay)
    {
        animator.SetBool("Jump", true);
        StartCoroutine(EndAnimation("Jump", delay));
    }

    public void SetClubSwing(int clubIndex, float delay)
    {
        animator.SetInteger("SwingClub", clubIndex);
        StartCoroutine(EndAnimation("SwingClub", -2, delay));
    }

    IEnumerator EndAnimation(string name, float delay)
    {
        yield return new WaitForSeconds(0.5f - delay);
        animator.SetBool(name, false);
    }

    IEnumerator EndAnimation(string name, int endIndex, float delay)
    {
        yield return new WaitForSeconds(0.5f - delay);
        animator.SetInteger(name, endIndex);
    }
}
