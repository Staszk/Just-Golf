using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RangeFinderItem : UsableItem
{
    private readonly float duration = 4.0f;
    public static event System.Action<float> EventRangeFinder = delegate { };

    //Item that shows locations of players
    public override void Use(PlayerController pc)
    {
        //Get players
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); local testing 
        GameObject[] players = GameObject.FindGameObjectsWithTag("Networked Player");

        if (NetworkManager.instance) { NetworkManager.instance.SendItemUseRequest(ItemType, null, null); }

        EventRangeFinder(duration);
        //Start Coroutine
        StartCoroutine(RunRadar());
       
        base.Use(pc);
    }

    IEnumerator RunRadar()
    {
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            InUse(1 - timer / duration);
            yield return null;
        }

        UseDone();
    }
}
