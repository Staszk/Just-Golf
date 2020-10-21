///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerGolf
///   Description:      
///   Author:           Parker Staszkiewicz
///   Contributor(s):   Mark Botaish
///-------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGolf : MonoBehaviour
{
    private PlayerController pc;

	private List<GolfBall> nearbyBalls = new List<GolfBall>();
	private List<GolfBall> hittableBalls;
    public GolfBall _GolfBall { get; private set; }
    private int activePower = 1; 
    private bool isGolfing;
   
    public bool CanGolf { get; private set; } = false;
    private float playerPositionOffSetMultiplier = 1;


    // Start is called before the first frame update
    void Start()
    {
        _GolfBall = null;
        pc = GetComponent<PlayerController>();

		hittableBalls = new List<GolfBall>();
    }

    public void LateUpdate()
    {
        if(!pc.IsDead)
            CheckGolfBall();
    }

    private void CheckGolfBall()
    {
		GolfBall closestBall = null;
		hittableBalls.Clear();

		if (nearbyBalls.Count > 0)
		{
			Vector3 transformedPosition = (transform.position) - (transform.right * playerPositionOffSetMultiplier) - (transform.forward * playerPositionOffSetMultiplier);

			for (int i = 0; i < nearbyBalls.Count; i++)
			{
                if (!nearbyBalls[i])
                    continue;

				float rightDot = Vector3.Dot(transform.right, (nearbyBalls[i].transform.position - transformedPosition).normalized);   //Determine if the ball is to the left                
				//float forwardDot = Vector3.Dot(transform.forward, (nearbyBalls[i].transform.position - transformedPosition).normalized); //Determine if the ball is to the right

				//if (!(rightDot <= 0 || forwardDot < -0.7f)) // Correct Position
                if(rightDot > 0f)
				{
					hittableBalls.Add(nearbyBalls[i]);
				}
			}

			if (hittableBalls.Count == 1)
			{
				closestBall = hittableBalls[0];
			}
			else if (hittableBalls.Count > 1)
			{
				closestBall = hittableBalls.OrderBy(element => (element.transform.position - transformedPosition).sqrMagnitude).ToArray()[0];
			}
		}

		if (closestBall)
		{
			if (closestBall != _GolfBall)
			{
				_GolfBall = closestBall;

				EventController.FireEvent(new NearbyGolfBallMessage(_GolfBall));	
			}

			CanGolf = true;
		}
		else
		{
			if (_GolfBall != null)
			{
				_GolfBall = null;

				EventController.FireEvent(new NearbyGolfBallMessage(null));

				//This is put here to avoid a bug found where the player will not be able to move
				//if the ball they are swinging at despawns mid swing
				EventController.FireEvent(new MakeSureThePlayerCanMoveMessage());
			}

			CanGolf = false;
		}
	}

    private void OnTriggerEnter(Collider other)
    {
		GolfBall gb = other.GetComponent<GolfBall>();

		if (gb)
		{ 
            if(gb is AbilityBall ability)
            {
                if (!ability.CheckInUse())
                {
                    return;
                }
            }
			nearbyBalls.Add(gb);
			ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
        }          
    }

    private void OnTriggerExit(Collider other)
    {
		GolfBall gb = other.GetComponent<GolfBall>();

        if (gb)
        {
			ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

			if (gb is AbilityBall ab)
				ab.StartCooldown();

			nearbyBalls.Remove(gb);
        }  
    }

    public void SetGolfMode(bool isGolfing)
    {
        this.isGolfing = isGolfing;
    }

    public void SetClubPower(int powerLevel)
    {
        if (powerLevel != activePower)
        {
            activePower = powerLevel;
        }
    }
}
