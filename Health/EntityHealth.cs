using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHaveHealth
{

    [SerializeField] protected HeadPlacement headPlacement = null;
    [SerializeField] protected GameObject shieldObject = null;
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float respawnDelay = 5.0f;
    protected float golfBallSpeedThreshold = 1.0f;
    
    protected NetworkManager network;
    protected NetworkStoredData nStoredDate;
    protected PlayerScoreTracker scoreTracker;

    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }
    public int MyID { get; set; } = -1;

    protected int lastShotID = 0;
    protected bool isShielded = false;

    protected virtual void Awake()
    {
        Health = maxHealth;
        IsDead = false;
        scoreTracker = GetComponent<PlayerScoreTracker>();
        nStoredDate = NetworkStoredData.instance;

        if (headPlacement != null)
        {
            GameObject go = new GameObject("Head Collider");
            go.transform.position = transform.position;
            HeadEntity newHead = go.AddComponent<HeadEntity>();
            newHead.SetUp(headPlacement, transform, this);
        }
    }

    protected virtual void Start()
    {
        network = NetworkManager.instance;
    }

    public void SimulateDamage(Vector3 worldPos, float damage)
    {
        // Show damage pop offs
       // GameplayUIController.instance.UsePopOff(new SimulatedDamageMessage(worldPos, damage, false));
    }

    public virtual void GainHealth(float amount)
    {
        Health = Mathf.Clamp(Health + amount, 1f, maxHealth);
    }

    public virtual void TakeDamage(float damage, int shooterID = -1) {     
    }

    public virtual bool CheckShield()
    {
        if (isShielded)
        {
            isShielded = false;
            shieldObject.SetActive(false);

            if (network && network.IsHost())
            {
                network.SendDamageMessage(gameObject, lastShotID);
            }

            return true;
        }
        return false;
    }

    public virtual void ResetPlayer(Vector3 pos)
    {
        IsDead = false;
        Health = maxHealth;
    }

    public virtual void Respawn(Vector3 location)
    {
        gameObject.transform.position = location;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GolfBall") && network && network.IsHost())
        {
            GolfBall ball = collision.gameObject.GetComponent<GolfBall>();
            
            if(ball.GetVelocity().sqrMagnitude >= golfBallSpeedThreshold * golfBallSpeedThreshold)
            {
                TakeDamage(ball.GetDamage(), ball.GetLastShotId());
            }
        }
    }

    public float GetHealth() { return Health; }
    public void TurnOnShield()
    {
        isShielded = true;
        shieldObject.SetActive(true);
    }

}
