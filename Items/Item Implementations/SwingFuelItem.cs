using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SwingFuelItem : UsableItem
{
    private readonly float duration = 6.0f;
    public static event System.Action<float> EventInk = delegate { };

    public override void Use(PlayerController pc)
    {
        if (!NetworkManager.instance)
        {
            base.VFXEffectEvent(ParticleEffectMessage.Effect.Inked);
            SoundManager.PlaySound("Ink");
            EventInk(duration);
        }
        else
        {
            NetworkManager.instance.SendItemUseRequest(ItemType, new List<float>() { duration }, null);
        }

        base.Use(pc);
        UseDone();
    }
}
