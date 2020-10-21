using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMPBall : AbilityBall
{
    [Header("AMP Ball Specific Variables")]
    public float speedMultiplier;
    public int damageMultiplier;
    public float rangeMultiplier;
    public float effectLength;

    public override void TriggerAbilityPlayer(Collider other)
    {
        
        if (!network || network.IsHost())
        {
            int id = other.GetComponent<EntityHealth>().MyID;
           
            if (network) {
                network.SendTriggerdAbilityEvent(id, GameEvent.ABILITY_EVENT_AMP, speedMultiplier, damageMultiplier, rangeMultiplier, effectLength);
            }
            else
            {
                EventController.FireEvent(new AMPBallMessage(id, speedMultiplier, damageMultiplier, rangeMultiplier));
                StartCoroutine(EndEffect(id));
            }
           
        
        }

        base.TriggerAbilityPlayer(other);
    }

    IEnumerator EndEffect(int id)
    {
        yield return new WaitForSeconds(effectLength);
        EventController.FireEvent(new EndAMPBallMessage(id));
    }
}
