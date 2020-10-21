using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : AbilityBall
{
    private float freezeDuration = 3f;
    public override void TriggerAbilityPlayer(Collider other)
    {
        TargetDummy t = other.GetComponent<TargetDummy>();
        EntityHealth enemy = other.GetComponent<EntityHealth>();
        
        if (t)
        {
            EventController.FireEvent(new WorldVFXMessage(WorldSpaceVFX.WorldVFXType.ice, t.transform.position, t.transform));
            t.FreezeDummy(freezeDuration);
        }

        if (enemy && network && network.IsHost())
        {
            EventController.FireEvent(new WorldVFXMessage(WorldSpaceVFX.WorldVFXType.ice, enemy.transform.position, enemy.transform));
            network.SendTriggerdAbilityEvent(enemy.MyID, GameEvent.ABILITY_EVENT_FREEZE, freezeDuration);
            network.SendWorldVFX(WorldSpaceVFX.WorldVFXType.ice, enemy.transform.position, enemy.MyID, parent.GetIdOfAbilityBall(this));
        }

        //Old Mehtod of using radius

        /*Collider[] cols = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in cols)
        {
            TargetDummy t = collider.GetComponent<TargetDummy>();

            Debug.Log("Hit: " + collider.name);

            if (t)
            {
                EventController.FireEvent(new WorldVFXMessage(WorldSpaceVFX.WorldVFXType.ice, t.transform.position, t.transform));
                t.FreezeDummy(freezeDuration);
            }
        }
        */
        base.TriggerAbility();
    }
}
