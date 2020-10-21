using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
	private ColoredBall.BallColor currentColor;

    private Renderer lightBeam;
    private GameObject flagPole;

    private float duration = .4f;
    private float yeetFactor = -1.5f;

    Coroutine dbsRoutine;

    //Lerping beam variables
    private float lerpA = 1f;
    private float lerpB = 10f;
    private float t = 0.0f;

    bool lerp = false;
    bool flip = false;

    public bool isActive = false;

    private NetworkManager network;
    private NetworkStoredData nStoredData;

    private HoleManager holeManager;

    private void Start()
    {
        lightBeam = gameObject.transform.parent.parent.GetChild(1).gameObject.GetComponent<Renderer>();
        flagPole = gameObject.transform.parent.GetChild(1).gameObject;
        network = NetworkManager.instance;
        nStoredData = NetworkStoredData.instance;

        holeManager = GameObject.FindGameObjectWithTag("HoleManager").GetComponent<HoleManager>();

        if (isActive)
        {
            ActivateHole();
        }
    }

    public void ActivateHole()
    {
        isActive = true;
        lightBeam.gameObject.SetActive(true);
        flagPole.SetActive(true);
    }

    public void DeactivateHole()
    {
        isActive = false;
        lightBeam.gameObject.SetActive(false);
        flagPole.SetActive(false);
    }

    public void CheckBallScore(GolfBall gb)
    {
        if (isActive)
        {
            //Ensure the ball is colored and not an ability ball
            if (gb is ColoredBall cb)
            {
                gb.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.Albatross,
                    SuperlativeController.ConditionFlag.identity, Vector3.Distance(transform.position, gb.GetComponent<BallRetrieval>().GetLastPos()),
                    gb.GetLastShotId()));

                if (!network || (network && network.IsHost()))
                {
                    StartCoroutine(BeamFlash());
                    if (dbsRoutine == null)
                    {
                        dbsRoutine = StartCoroutine(DelayedBallDespawn(gb, duration));
                        SoundManager.PlaySoundAt("Ball Scored", transform.position);
                        SoundManager.PlaySound("Golf Clap");
                    }
                }

                //For tutorial purposes
                if (PlayerTutorialChecker.instance != null)
                {
                    PlayerTutorialChecker.instance.BallScored();
                }
                return;

            }
            else if (gb is AbilityBall ab)
            {
                ab.GetComponent<Rigidbody>().velocity *= yeetFactor;
            }
        }
        else
        {
            gb.GetComponent<Rigidbody>().velocity *= yeetFactor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
  //      Debug.Log(other.name);
		//GolfBall gb = other.GetComponentInParent<GolfBall>();

  //      if (gb)
  //      {
  //          //Reject the ball if the hole isn't active
  //          if (isActive)
  //          {
  //              //Ensure the ball is colored and not an ability ball
  //              if (gb is ColoredBall cb)
  //              {
  //                  gb.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
  //                  EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.Albatross, 
  //                      SuperlativeController.ConditionFlag.identity, Vector3.Distance(transform.position, gb.GetComponent<BallRetrieval>().GetLastPos()), 
  //                      gb.GetLastShotId()));

  //                  if (!network || (network && network.IsHost()))
  //                  {
  //                      StartCoroutine(BeamFlash());
  //                      StartCoroutine(DelayedBallDespawn(gb, duration));
  //                      SoundManager.PlaySoundAt("Ball Scored", transform.position);
  //                      SoundManager.PlaySound("Golf Clap");
  //                  }

  //                  //For tutorial purposes
  //                  if (PlayerTutorialChecker.instance != null)
  //                  {
  //                      PlayerTutorialChecker.instance.BallScored();
  //                  }
  //                  return;

  //              }
  //              else if (gb is AbilityBall ab)
  //              {
  //                  ab.GetComponent<Rigidbody>().velocity *= yeetFactor;
  //              }
  //          }
  //          else
  //          {
  //              gb.GetComponent<Rigidbody>().velocity *= yeetFactor;
  //          }
  //      }
    }

    private IEnumerator DelayedBallDespawn(GolfBall gb, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        int team = nStoredData ? nStoredData.GetTeam(gb.GetLastShotId()) : 0;
        if (network && network.IsHost())
        {
            EventController.FireEvent(new PersonalScoreMessage(gb.GetLastShotId(), PersonalScoreManager.TYPE.SCORE_BALL));
            gb.EndLifetime();
            EventController.FireEvent(new NetworkSendBallRespawn(gb, team));           
        }

        if (!network)
        {
            gb.EndLifetime();
            EventController.FireEvent(new TeamScoreMessage());
        }
   
        holeManager.ChangeHole();
        dbsRoutine = null;

    }

    private void Update()
    {
        if (lerp)
        {
            LerpBeam();
            Debug.Log(lightBeam.material.GetFloat("_AlphaBoost"));
        }
    }

    private IEnumerator BeamFlash()
    {
        lerp = true;
        yield return new WaitForSeconds(.25f);
        lerp = false;
        flip = true;
        t = 0.0f;
        lerp = true;
        yield return new WaitForSeconds(.25f);
        lerp = false;
        flip = false;
        lightBeam.material.SetFloat("_AlphaBoost", lerpA);
    }

    private void LerpBeam()
    {
        if (!flip)
        {
            lightBeam.material.SetFloat("_AlphaBoost", Mathf.Lerp(lerpA, lerpB, t));
            t += Time.deltaTime * 4;
            if (t > 1.0f)
               t = 1.0f;
        }
        else
        {
            lightBeam.material.SetFloat("_AlphaBoost", Mathf.Lerp(lerpB, lerpA, t));
            t += Time.deltaTime * 4;
            if (t > 1.0f)
               t = 1.0f;
        }

    }



}
