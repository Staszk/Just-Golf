using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PortableSpeakerItem : UsableItem
{
    private readonly float blastDistance = 30.0f;
    private readonly float stunTime = 1.5f;
    private readonly float visualTime = .5f;

    public GameObject wavePrefab; // Needed to show visual of ray for now

    public override void Use(PlayerController pc)
    {
        Vector3 dir = pc.gameObject.transform.forward;
        Vector3 startPos = pc.gameObject.transform.position + (pc.gameObject.transform.forward * 2);

        if (NetworkManager.instance)
        {
            NetworkManager.instance.SendPortableSpeakers(dir,startPos, blastDistance, stunTime, visualTime);
        }

        //Create visual for waves (please replace with actual art later)
        GameObject visual = Instantiate(wavePrefab);
        visual.transform.position = pc.gameObject.transform.position + (pc.gameObject.transform.forward * (blastDistance));
        visual.transform.rotation = pc.gameObject.transform.rotation;

        //Destroy it after a short time to show where it was fired
        Destroy(visual, visualTime);

        base.Use(pc);
    }
}
