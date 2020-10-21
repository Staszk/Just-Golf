using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBall : AbilityBall
{
    private static int ballsAvailable;
    private static float cooldownCounter;

    public override void TriggerAbilityPlayer(Collider other)
    {
        EntityHealth enemy = other.gameObject.GetComponent<EntityHealth>();
        if (enemy)
        {
            enemy.TakeDamage(damage, IDofLastHit);
            if (enemy.GetHealth() - damage <= 0)
            {
                EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.HailMary,
                        SuperlativeController.ConditionFlag.identity, Vector3.Distance(transform.position, GetComponent<BallRetrieval>().GetLastPos()),
                        GetLastShotId()));
            }
        }

        base.TriggerAbility();
    }

    public override void Initialize(GolfBallManager gbm)
    {
        ballsAvailable = totalBallsAvailable;
        cooldownCounter = cooldown;
        base.Initialize(gbm);
    }

    public override void TurnOn()
    {
        if (ballsAvailable > 0)
        {
            --ballsAvailable;

            if (ballsAvailable == totalBallsAvailable - 1)
                cooldownCounter = 0;

            gameObject.SetActive(true);
            inUse = true;
            hit = false;
            time = 0;
        }
    }

    public override float CheckCooldown()
    {
        if (ballsAvailable < totalBallsAvailable)
        {
            cooldownCounter += Time.deltaTime;

            if (cooldownCounter >= cooldown)
            {
                ++ballsAvailable;

                if (ballsAvailable < totalBallsAvailable)
                    cooldownCounter = 0;
            }
        }
        return (cooldownCounter / cooldown);
    }

    public void EditDamage(int multiplier)
    {
        damage *= multiplier;
    }

    public void ChangeDamageBackToOriginal()
    {
        damage = 25;
    }
}
