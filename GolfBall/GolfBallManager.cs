using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallManager : EventListener
{
    [SerializeField] private Material[] coloredBallMats = null;

    [SerializeField] private Transform[] spawnLocations = null;

    [SerializeField] private ColoredBall golfBallPrefab = null;
    [SerializeField] private int ballCount = 20;
    private int totalBalls;

    [Header("Keep this in the order of the enum")]
    [SerializeField] private AbilityBall[] abilityBallPrefabs = null;

    private ColoredBall[] objectiveBalls;
    private List<AbilityBall> abilityBalls;

    private Transform lastAbilityBallSpawned;   //Store the transform of the last ability ball spawned to check if we are far enough away from it to spawn another

    private NetworkManager network;

    private int[] currentAbilityIndex = new int[4]; //For initializing ability balls
    int startIndexForAbility = 0;
    int endIndexForAbility = 3;
    int id = 0;
    int maxNumberOfAbilityBalls = 7;

    private void Awake()
    {
        abilityBalls = new List<AbilityBall>(new AbilityBall[28]);
        PrepareBalls();
        StartCoroutine(SpawnAllColoredBalls());
    }


    private void Start()
    {
        network = NetworkManager.instance;
        id = network ? network.GetId() : 0;
        startIndexForAbility = id * maxNumberOfAbilityBalls;// 2 ability balls + 5 storage balls
        endIndexForAbility = startIndexForAbility + maxNumberOfAbilityBalls;
    }

   
    private void OnEnable()
    {
        EventController.AddListener(typeof(AbilityBallPrepareMessage), this);
        EventController.AddListener(typeof(InitializeAbilityBall), this);
        EventController.AddListener(typeof(NetworkAbilityBallMessage), this);
        EventController.AddListener(typeof(DeactivateBall), this);
        EventController.AddListener(typeof(AMPBallMessage), this);
        EventController.AddListener(typeof(EndAMPBallMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(AbilityBallPrepareMessage), this);
        EventController.RemoveListener(typeof(InitializeAbilityBall), this);
        EventController.RemoveListener(typeof(NetworkAbilityBallMessage), this);
        EventController.RemoveListener(typeof(DeactivateBall), this);
        EventController.RemoveListener(typeof(AMPBallMessage), this);
        EventController.RemoveListener(typeof(EndAMPBallMessage), this);
    }

    private void Update()
    {
        UpdateCoolDown();
    }

    private void UpdateCoolDown()
    {
        int index = 0;
        if(!network || network.IsHost())
        {
            for (int i = 0; i < abilityBalls.Count; ++i)
            {
                if (abilityBalls[i])
                {
                    float cooldownCounter = abilityBalls[i].CheckCooldown();
                    if (i >= startIndexForAbility && i < startIndexForAbility + 3)
                    {
                        EventController.FireEvent(new AbilityCooldownMessage(index, cooldownCounter));
                        index++;
                    }
                }                         
            }
        }
        else
        {
            for (int i = startIndexForAbility; i < startIndexForAbility + 3; ++i)
            {
                float cooldownCounter = abilityBalls[i].CheckCooldown();
                EventController.FireEvent(new AbilityCooldownMessage(index, cooldownCounter));
                index++;
            }
        }
       
    }

    private void PrepareBalls()
    {
        //int numColors = (int)ColoredBall.BallColor.Count;
        totalBalls = ballCount;
        objectiveBalls = new ColoredBall[totalBalls];

        int i = 0;

        //for (int i = 0; i < numColors; i++)
        //{
            for (int j = 0; j < ballCount; j++)
            {
                ColoredBall gb = Instantiate(golfBallPrefab, transform);
                gb.Initialize(this);
                gb.gameObject.SetActive(false);
                gb.BecomeColor((ColoredBall.BallColor)i, coloredBallMats[i]);
                objectiveBalls[(i * ballCount) + j] = gb;
            }
        //}

        // Send Message
        EventController.FireEvent(new ObjectBallsSpawnedMessage(objectiveBalls));
    }

    private void SpawnAbilityBalls(AbilityBall.Abilities ability, int id)
    {
       
        int ballsToSpawn = 1;
        if (ability == AbilityBall.Abilities.storageBall)
            ballsToSpawn = 5;
        int index = id * maxNumberOfAbilityBalls + currentAbilityIndex[id];
        for (int i = 0; i < ballsToSpawn; ++i)
        {
            AbilityBall obj = Instantiate(abilityBallPrefabs[(int)ability]);
            obj.Initialize(this);
            obj.Despawn();
            abilityBalls[index] = obj;
            currentAbilityIndex[id]++;
            index++;
        }
    }

    private IEnumerator SpawnAllColoredBalls()
    {
        if (spawnLocations != null)
        {
            //if (spawnLocations.Length < totalBalls)
            //{
            //    Debug.Log("Not enough unique spawn points.");

            //    for (int i = 0; i < totalBalls; i++)
            //    {
            //        SpawnBallAt(objectiveBalls[i], spawnLocations[i % spawnLocations.Length].position, i);

            //        yield return new WaitForSeconds(0.15f);
            //    }

            //}
            //else
            {
                bool flag = false;
                for (int i = 0; i < totalBalls; i++)
                {
                    if (i % (ballCount * 0.5f) == 0)
                        flag = !flag;

                    if (flag)
                        SpawnBallAt(objectiveBalls[i], spawnLocations[0].position, i);
                    else
                        SpawnBallAt(objectiveBalls[i], spawnLocations[0].position, i);

                    yield return new WaitForSeconds(0.075f);
                }
            } 
        }
    }

    private void SpawnBallAt(GolfBall gb, Vector3 pos, int index)
    {
        gb.gameObject.SetActive(true);
        float intenseMath = Mathf.Abs(-((2 * index) % 10) + 5) * 2;

        Vector3 variation = new Vector3(intenseMath * Mathf.Sin(index), 0, intenseMath * Mathf.Cos(index));

        gb.transform.position = pos + variation;
        gb.Prepare();
    }

    public void RespawnBall(GolfBall gb)
    {
        Debug.Log("Previous: " + gb.transform.position);
        gb.transform.position = transform.position;

        int randomIndex = UnityEngine.Random.Range(0, spawnLocations.Length);
        SpawnBallAt(gb, spawnLocations[randomIndex].position, randomIndex);
        Debug.Log("New: " + gb.transform.position);
    }

    public void RespawnBallAtLocation(GolfBall gb, int locationIndex)
    {
        SpawnBallAt(gb, spawnLocations[locationIndex].position, locationIndex);
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is AbilityBallPrepareMessage abilityMessage)
        {

            for (int i = startIndexForAbility; i < endIndexForAbility; i++)
            {
                if (abilityBalls[i].Ability == abilityMessage.ability && !abilityBalls[i].CheckInUse())
                {
                    //If the last ability ball spawned isn't null and the player is far enough away 
                    //from it or if the variable is null, then we can spawn the ball
                    if ((lastAbilityBallSpawned && Vector3.Distance(abilityMessage.player.transform.position, lastAbilityBallSpawned.position) > 1)
                        || !lastAbilityBallSpawned)
                    {
                        RaycastHit hit;
                       
                        //Shoot a raycast pointing downwards from above where the ball should actually go
                        Vector3 raycastStartPos = abilityMessage.player.transform.position +
                            (abilityMessage.player.transform.right * 0.5f)
                            + (abilityMessage.player.transform.forward * 0.75f)
                            + (abilityMessage.player.transform.up * 5);

                        Physics.Raycast(raycastStartPos, -Vector3.up, out hit);

                        //Place the ball wherever the raycast hit plus a bit in the y so that it doesn't spawn into the floor
                        abilityBalls[i].transform.position = hit.point + new Vector3(0, abilityBalls[i].transform.localScale.y, 0);
                        lastAbilityBallSpawned = abilityBalls[i].transform;
                        abilityBalls[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                        if (network) { network.SendAbilityBall(i, abilityBalls[i].transform.position); }
                        abilityBalls[i].TurnOn();

                        // Ability Ball is Used
                        EventController.FireEvent(new AbilityBallUseMessage(abilityMessage.slotID));

                        break;
                    }
                }
            }
        }
        else if(e is InitializeAbilityBall initMessage)
        {
            SpawnAbilityBalls(initMessage.ability, initMessage.id);
        }else if(e is NetworkAbilityBallMessage message)
        {
            abilityBalls[message.index].GetComponent<Rigidbody>().velocity = Vector3.zero;
            abilityBalls[message.index].transform.position = message.position;
            abilityBalls[message.index].TurnOn();
        }else if (e is DeactivateBall ball)
        {
            abilityBalls[ball.id].Despawn();
        }
        else if (e is AMPBallMessage amp)
        {
            IncreaseStorageBallDamage(amp.damageMultiplier);
        }
        else if (e is EndAMPBallMessage endAmp)
        {
            ChangeStorageBallDamageBackToOriginal();
        }
    }

    private void IncreaseStorageBallDamage(int multiplier)
    {
        foreach(AbilityBall a in abilityBalls)
        {
            if(a is StorageBall s)
                s.EditDamage(multiplier);
        }
    }

    private void ChangeStorageBallDamageBackToOriginal()
    {
        foreach (AbilityBall a in abilityBalls)
        {
            if (a is StorageBall s)
                s.ChangeDamageBackToOriginal();
        }
    }

    public GameObject GetBallOfId(int id) { return objectiveBalls[id].gameObject; }
    public GameObject GetAbilityBallOfId(int id) { return abilityBalls[id].gameObject; }
    public int GetIdOfAbilityBall(AbilityBall ball) { return abilityBalls.IndexOf(ball); }
    public int GetIdOfBall(ColoredBall ball) {
        for(int i  = 0; i < objectiveBalls.Length; i++)
        {
            if(objectiveBalls[i] == ball)
            {
                return i;
            }
        }
        return -1;
    }
}
