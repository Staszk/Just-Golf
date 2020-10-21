using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("ProcessorManager")]
public class NetworkBallManager : EventListener
{
    private int _myID;
    private NetworkManager network = null;

    private List<GolfBall> activeBalls = new List<GolfBall>();
    private List<GolfBall> MovingAbilityBalls = new List<GolfBall>();

    internal GolfBallManager golfBallManager = null;

    public GameObject GetBallOfID(int id) { return golfBallManager.GetBallOfId(id); }
    public GameObject GetAbilityBallOfID(int id) { return golfBallManager.GetAbilityBallOfId(id); }

    private void OnEnable()
    {
        EventController.AddListener(typeof(NetworkSendBallRespawn), this);
        EventController.AddListener(typeof(ObjectBallsSpawnedMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(NetworkSendBallRespawn), this);
        EventController.RemoveListener(typeof(ObjectBallsSpawnedMessage), this);
    }

    private void Start()
    {
        network = NetworkManager.instance;

        if (!network) { return; }

        _myID = network.GetId();
        golfBallManager = GameObject.Find("Golf Ball Manager")?.GetComponent<GolfBallManager>();
    }

    private void Update()
    {
        if (network != null && network.IsHost())
        {
            for (int i = 0; i < activeBalls.Count; i++)
            {
                network.SendBallMessage(activeBalls[i].gameObject, activeBalls[i].transform.GetSiblingIndex());
            }
            
            MovingAbilityBalls.RemoveAll(ball => ball.IsStationary());
            for (int i = 0; i < MovingAbilityBalls.Count; i++)
            {
                network.SendBallMessage(MovingAbilityBalls[i].gameObject, golfBallManager.GetIdOfAbilityBall(MovingAbilityBalls[i] as AbilityBall), true);
            }
        }        
    }

    public override void HandleEvent(EventMessage e)
    {
       if (e is ObjectBallsSpawnedMessage respawn)
        {            
            for(int i = 0; i < respawn.ballsArray.Length; i++)
            {
                respawn.ballsArray[i].GetComponent<Rigidbody>().velocity = Vector3.up * 10f;
            }
            activeBalls = new List<GolfBall>(respawn.ballsArray);
        }
    }

    /// <summary>
    /// Hits the ball on the host.
    /// </summary>
    /// <param name="code">The message data from the packets.</param>
    internal void UpdateBalls(string code)
    {
        SerializationScript.BallShot ballShot = SerializationScript.DeserializeBallShot(code);
        GameObject ball = ballShot.isAbilityBall ? golfBallManager.GetAbilityBallOfId(ballShot.ballID) : golfBallManager.GetBallOfId(ballShot.ballID);
        BallRetrieval br = ball.GetComponent<BallRetrieval>();
        ball.GetComponent<Rigidbody>().velocity = ballShot.direction;
        if(br)
            ball.GetComponent<BallRetrieval>().SetNewBallPos();
        GolfBall golf = ball.GetComponent<GolfBall>();

        golf.HitBall(ballShot.shooterID, network.IsHost());
        if (network.IsHost()) {
            if (golf.IsAbilityBall()) { MovingAbilityBalls.Add(golf); }

            if (ballShot.shouldHome){
                Honing theHone = ball.gameObject.GetComponent<Honing>();
                if(theHone != null && ballShot.homingTargetID >= 0)
                {
                    theHone.HoneToPlayer(network.GetPlayerOfID(ballShot.homingTargetID));
                }
              
               
            }
        }

        
    }

    /// <summary>
    /// Deadreckons and updates veloctiy of a specific ball
    /// </summary>
    /// <param name="code">The message data from the packets.</param>
    /// <param name="delay">The delay between sending and recieving packets.</param>
    internal void UpdateBallMovement(string code, int delay)
    {
        string[] splitCode = code.Split(':');
        int id = int.Parse(splitCode[1]);

        GameObject ball = bool.Parse(splitCode[splitCode.Length - 1]) ? GetAbilityBallOfID(id) : GetBallOfID(id);

        if (!ball)
            return;

        Vector3 movement = SerializationScript.StringToVector3(splitCode[2]);
        float delayInSeconds = ((float)delay / 1000.0f);
        float GravityConstant = ball.GetComponent<BetterGravity>().GetGravityConstant().y;

        float x = movement.x * delayInSeconds;
        float z = movement.z * delayInSeconds;
        float y = movement.y * delayInSeconds + 0.5f * GravityConstant * delayInSeconds * delayInSeconds;
        ball.transform.position += new Vector3(x, y, z);
        movement.y = movement.y + GravityConstant * delayInSeconds;
        ball.GetComponent<Rigidbody>().velocity = movement;
        ball.GetComponent<BetterGravity>().ShouldUpdateGravity = false;
    }
}
