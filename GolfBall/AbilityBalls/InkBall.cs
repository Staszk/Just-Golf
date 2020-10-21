using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBall : AbilityBall
{

    public override void TriggerAbility()
    {
        if (!NetworkManager.instance)
        {
            EventController.FireEvent(new ParticleEffectMessage(ParticleEffectMessage.Effect.Inked, IDofLastHit));
            SoundManager.PlaySound("Ink");
        }
        
        base.TriggerAbility();

    }
}
