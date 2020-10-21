using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusedControlItem : UsableItem
{
    private readonly float duration = 6.0f;
    public static event System.Action EventConfused = delegate { }; //For testing locally
 
    //Item that shows locations of players
    public override void Use(PlayerController pc)
    {

        if (NetworkManager.instance) { NetworkManager.instance.SendItemUseRequest(ItemType, new List<float>() { duration }, null); }
        else{
            EventConfused();
            base.VFXEffectEvent(ParticleEffectMessage.Effect.Confused);
            SoundManager.PlaySound("Confused");
        }

        //Start Coroutine
        StartCoroutine(ConfusedTime());

        base.Use(pc);
    }

    IEnumerator ConfusedTime()
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            InUse(1 - timer / duration);
            yield return null;
        }

        if(!NetworkManager.instance)
            EventConfused();

        UseDone();
    }
}
