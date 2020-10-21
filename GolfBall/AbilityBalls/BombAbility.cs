using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAbility : AbilityBall
{
    public float firstRadius = 4f;
    public float secondRadius = 8f;
    public float lastRadius = 10f;

    public override void TriggerAbilityPlayer(Collider other)
    {
        DoAbility(other);
    }

    public override void TriggerAbility()
    {
        DoAbility(null);
    }

    private void DoAbility(Collider other)
    {
        if ((network && network.IsHost()) || !network)
        {
            Vector3 position = other ? other.gameObject.transform.position : transform.position;
            EventController.FireEvent(new WorldVFXMessage(WorldSpaceVFX.WorldVFXType.bomb, position));
            if (network) { network.SendWorldVFX(WorldSpaceVFX.WorldVFXType.bomb, position, -1, parent.GetIdOfAbilityBall(this)); }
            //First radius
            Collider[] cols = Physics.OverlapSphere(transform.position, lastRadius);

            foreach (Collider collider in cols)
            {
                TargetDummy t = collider.GetComponent<TargetDummy>();
                EntityHealth enemy = collider.GetComponent<EntityHealth>();

                float sqrDist = (collider.transform.position - transform.position).sqrMagnitude;
                if (sqrDist <= firstRadius * firstRadius)
                {
                    if (t) { t.ShowHit(75); }
                    else if (enemy) { DoBombDamage(enemy, 75); }
                }
                else if (sqrDist <= secondRadius * secondRadius)
                {
                    if (t) { t.ShowHit(50); }
                    else if (enemy) { DoBombDamage(enemy, 50); }
                }
                else if (sqrDist <= lastRadius * lastRadius)
                {
                    if (t) { t.ShowHit(50); }
                    else if (enemy) { DoBombDamage(enemy, 50); }
                }
            }

            SoundManager.PlaySoundAt("Bomb Explosion", transform.position);
        }

        base.TriggerAbility();
    }

    private void DoBombDamage(EntityHealth enemy, int damage)
    {
        enemy.TakeDamage(damage, IDofLastHit);
        if (enemy.GetHealth() - damage <= 0)
        {
            EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.HailMary,
            SuperlativeController.ConditionFlag.identity, Vector3.Distance(transform.position, GetComponent<BallRetrieval>().GetLastPos()),
            GetLastShotId()));
        }
    }
}
