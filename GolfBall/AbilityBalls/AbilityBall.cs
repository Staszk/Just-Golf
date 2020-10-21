using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBall : GolfBall
{
    //ABILITY-EDIT
    public enum Abilities
    {
        storageBall,
        bombBall,
        iceBall,
        shieldBall,
        AMPBall,
		none
    }

    [SerializeField] protected float cooldown = 0;
	[SerializeField] protected float despawnTime = 0;
	[SerializeField] protected int totalBallsAvailable = 1;
	[SerializeField] private Abilities ability = Abilities.none;
	public Abilities Ability { get { return ability; } }

    [Header("Conditions To Trigger Ability")]
    [SerializeField] protected bool collideWithEnemy = false;
    [SerializeField] protected bool collideWithAlly = false;
    [SerializeField] protected bool colliderWithWorld = false;
	[SerializeField] protected bool stopsRolling = false;

    protected float time = 0;
    private float cooldownCounter;
    protected bool inUse = false;
    private bool startCooldown = false;

    private int ballsAvailable;

    public override void Initialize(GolfBallManager gbm)
    {
        ballsAvailable = totalBallsAvailable + 1;
        cooldownCounter = cooldown;
        isAbilityBall = true;
        base.Initialize(gbm);
    }

    void Update()
    {
        if (inUse)
        {
            if (hit || startCooldown)
                CheckDespawn();

            if (hit && stopsRolling)
                CheckIfBallStopped();           
               
        }
    }

    private void CheckDespawn()
    {
        time += Time.deltaTime;

        if (time >= despawnTime)
        {
            if (!hit)
                TriggerAbility();
            else
                Despawn();
        }
    }

    private void CheckIfBallStopped()
    {
        if (IsStationary() && time > 0.08f)
            TriggerAbilityPlayer(null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Networked Player") || other.CompareTag("Player"))  && collideWithEnemy && hit)
            TriggerAbilityPlayer(other);
        if ((other.CompareTag("Networked Player") || other.CompareTag("Player")) && collideWithAlly && hit)
            TriggerAbilityPlayer(other);
        else if (!other.CompareTag("Terrain") && colliderWithWorld && hit)
            TriggerAbilityPlayer(other);
    }

    public virtual void TriggerAbility()
    {
        Despawn();
    }

    public virtual void TriggerAbilityPlayer(Collider other)
    {
        Despawn();
    }

    public virtual void Despawn()
    {
        if(!hit)
            EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.Hoarder, SuperlativeController.ConditionFlag.additive, 1));

        --ballsAvailable;
        hit = false;
        inUse = false;
        time = 0;
        rgbd.velocity = Vector3.zero;
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }

    public virtual void TurnOn()
    {
        if (ballsAvailable > 0)
        {
            //--ballsAvailable;
            cooldownCounter = 0;
            gameObject.SetActive(true);
            inUse = true;
            hit = false;
            time = 0;
        }
    }


    public virtual void NetworkTurnOn()
    {
        cooldownCounter = 0;
        gameObject.SetActive(true);
        inUse = true;
        hit = false;
        time = 0;
    }

    public virtual float CheckCooldown()
    {
        if (ballsAvailable < totalBallsAvailable && !inUse)
        {
            cooldownCounter += Time.deltaTime;

            if (cooldownCounter >= cooldown)
            {
                ++ballsAvailable;
            }
        }
        else if (inUse)
        {
            return 1;
        }

        return (cooldownCounter / cooldown);
    }

    public bool CheckInUse()
    {
        return inUse;
    }

    public void StartCooldown()
    {
        startCooldown = true;
    }
}
