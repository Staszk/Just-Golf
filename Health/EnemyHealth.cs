using System.Collections;
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    public override void TakeDamage(float damage, int shooterID = 0)
    {
        if (CheckShield()) { return; }

        if (!IsDead && shooterID != MyID)
        {
            lastShotID = shooterID;
            Health -= damage;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            SoundManager.PlaySound("Hit Marker");
            if (network && network.IsHost())
            {
                network.SendDamageMessage(gameObject, lastShotID);
            }
            
            if (Health <= 0) {
                IsDead = true;
                Die();
                EventController.FireEvent(new PersonalScoreMessage(lastShotID, PersonalScoreManager.TYPE.KILL_PLAYER));
            }
        }
    }


    private void Die()
    {
        if (NetworkManager.instance.IsHost()) { RespawnManager.instance.Death(gameObject, respawnDelay); }
        else { RespawnManager.instance.Death(gameObject); print("IM DEAD BITCH"); }        
    }

    public override void ResetPlayer(Vector3 pos)
    {
        base.ResetPlayer(pos);
        Respawn(pos);
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }
}
