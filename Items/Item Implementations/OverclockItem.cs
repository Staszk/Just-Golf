using UnityEngine;
using System.Collections;

public class OverclockItem : UsableItem
{
    public static event System.Action<EventMessage> EventToggleOverclock = delegate { };

    private float totalTime = 20f;

    public override void Use(PlayerController pc)
    {
        base.Use(pc);

        EventToggleOverclock(new EventMessage());

        StartCoroutine(Using());
    }

    private IEnumerator Using()
    {
        float time = 0;
        float maxTime = totalTime;

        while (time != maxTime)
        {
            time = Mathf.Min(time, maxTime);

            InUse(1 - time / maxTime);

            yield return null;
        }

        EventToggleOverclock(new EventMessage());
        UseDone();
    }
}
