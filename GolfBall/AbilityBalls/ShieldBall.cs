using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBall : AbilityBall
{

    public override void TriggerAbilityPlayer(Collider other)
    {

      
        //If ally, give shields
        TargetDummy td = other?.GetComponent<TargetDummy>();
        if (td)
        {
            td.TurnOnShield();
        }
        else
        {
            if (!network || network.IsHost())
            {
                EntityHealth entity = other?.GetComponentInParent<EntityHealth>();
                if (entity)
                {
                    int id = entity.MyID;
                    EventController.FireEvent(new SendShieldMessage(id));                  
                }
            }              
        }

        base.TriggerAbilityPlayer(other);
    }
}
