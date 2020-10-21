using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GolfClub : MonoBehaviour
{
    [SerializeField] private ClubStat clubStats = null;
    public ClubStat ClubStats { get { return clubStats; } }

    // General
    // Club Variables
    private float powerOnRelease;
    private float powerOscillation;
    private bool swing = false;
    public bool IsPrepped { get; private set; } = false;

    public int MyID { get; private set; }

    public void Initialize(int id) { MyID = id; }

    public void EditMaxDistance(int id, float multiplier)
    {
        if(id == MyID)
            clubStats.IncreaseMaxDistance(multiplier);
    }

    public void ChangeMaxDistanceBackToOriginal(int id)
    {
        if (id == MyID)
            clubStats.ChangeMaxDistanceBackToOriginal();
    }

    #region General

    #endregion

    #region Club Functions

    public void PrepareSwing()
    {
        powerOnRelease = 0;
        powerOscillation = 0;
        IsPrepped = true;
    }

    public float ChargePower()
    {
        if (IsPrepped)
        {
            powerOscillation += Time.deltaTime;

            powerOnRelease = Mathf.PingPong(powerOscillation, clubStats.TimeForMaxDistance);

            EventController.FireEvent(new GolfChargeMessage(powerOnRelease / clubStats.TimeForMaxDistance));

            return Mathf.Lerp(clubStats.MinDistance, clubStats.MaxDistance, powerOnRelease / clubStats.TimeForMaxDistance);
        }

        return clubStats.MinDistance;
    }

    public void GolfSwing(GolfBall golfball)
    {
        if (!golfball || !IsPrepped)
            return;

        BallRetrieval br = golfball.GetComponent<BallRetrieval>();

        float power = Mathf.Lerp(clubStats.MinDistance, clubStats.MaxDistance, powerOnRelease / clubStats.TimeForMaxDistance);
        if(br)
            golfball.GetComponent<BallRetrieval>().SetNewBallPos();
        Vector3 dir = transform.root.forward;
        dir = Vector3.RotateTowards(dir, new Vector3(dir.x, dir.y + clubStats.VectorYIncrease, dir.z), 1, 0.0f);
        dir = dir.normalized * power;

        if (!NetworkManager.instance)
        {
            StartCoroutine(NoNetworkingWaitForSwing(golfball, dir));       
        }
        else
        {
            int id = golfball.IsAbilityBall() ? -1 : golfball.gameObject.transform.GetSiblingIndex();

            int targetID = -1;
            Honing theHone = golfball.gameObject.GetComponent<Honing>();
            if (clubStats.ClubName == "Driver" && theHone != null){
                targetID = theHone.GetIDOfClosestPlayer();
            }

            StartCoroutine(NetworkingWaitForSwing(golfball.gameObject, id, dir, golfball.IsAbilityBall(), clubStats.ClubName == "Driver", targetID));
        }
        StartCoroutine(WaitForSwing(golfball, clubStats, power));
    }

    IEnumerator WaitForSwing(GolfBall golfball, ClubStat clubStat, float power)
    {
        while (!swing)
        {
            yield return new WaitForEndOfFrame();
        }

        if (clubStats.ClubName == "Putter")
        {
            SoundManager.PlaySoundAt("Putter Hit", golfball.transform.position);
        }
        if (clubStats.ClubName == "Wedge")
        {
            SoundManager.PlaySoundAt("Wedge Hit", golfball.transform.position);
            SoundManager.PlaySoundAt("Golf Swing", golfball.transform.position);
        }
        if (clubStats.ClubName == "Driver")
        {
            SoundManager.PlaySoundAt("Driver Hit", golfball.transform.position);
            SoundManager.PlaySoundAt("Golf Swing", golfball.transform.position);
        }

        EventController.FireEvent(new GolfStrokeMessage(power));
        golfball.HitBall(MyID, NetworkManager.instance == null);
        EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.TheHurricane, SuperlativeController.ConditionFlag.additive, 1));

        //For tutorial
        if (PlayerTutorialChecker.instance != null)
        {
            PlayerTutorialChecker.instance.ClubWasHit(clubStats.ClubName);
        }
        EndShot();
    }

    IEnumerator NetworkingWaitForSwing(GameObject golfBall, int id, Vector3  dir, bool isAbilityBall, bool shouldHome, int targetID)
    {
        while(!swing)
        {
            yield return new WaitForEndOfFrame();
        }

        NetworkManager.instance.SendRequestToHitBall(golfBall, id, dir, isAbilityBall, shouldHome, targetID);
    }

    IEnumerator NoNetworkingWaitForSwing(GolfBall golfBall, Vector3 dir)
    {
        while (!swing)
        {
            yield return new WaitForEndOfFrame();
        }

        golfBall.GetComponent<Rigidbody>().velocity = dir;
        if (clubStats.ClubName == "Driver")
        {
            Honing theHone = golfBall.gameObject.GetComponent<Honing>();
            GameObject obj = theHone?.GetObjOfClosestPlayer();
            if (obj != null) { theHone.HoneToPlayer(obj); }
        }
    }

    public void EndShot()
    {
        powerOnRelease = 0;
        powerOscillation = 0;
        IsPrepped = false;
        swing = false;

        EventController.FireEvent(new GolfChargeMessage(powerOnRelease / clubStats.TimeForMaxDistance));
    }

    public void Swing()
    {
        swing = true;
    }

    #endregion
}
