using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperlativeController : EventListener
{
    public enum Superlative
    {
        Albatross, 
        HailMary,
        Enduring,
        TheBouncer,
        TheHurricane,
        TheDeadGuy,
        Hoarder,
        BreakingTheGame,
        SIZE_OF_SUPERLATIVES
    }

    public enum ConditionFlag { additive, identity }

    private float[][] superlativeTracker = null;
    bool isHost = false;
    int clientIndex;

    private void Start()
    {
        if (NetworkManager.instance == null)
        {
            Destroy(gameObject);
            return;
        }


        isHost = NetworkManager.instance.IsHost();
        clientIndex = NetworkManager.instance.GetId();

        superlativeTracker = new float[(int)Superlative.SIZE_OF_SUPERLATIVES][] {
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f },
            new float[4] { 0f, 0f, 0f, 0f }
        };
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(TrackSuperlativeMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(TrackSuperlativeMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is TrackSuperlativeMessage tracker)
        {
            if (isHost)
            {
                float[] arr = superlativeTracker[(int)tracker.superlative];

                if (tracker.id < 0) tracker.id = clientIndex;

                switch (tracker.condition)
                {
                    case ConditionFlag.additive:
                        arr[tracker.id] += tracker.value;
                        break;
                    case ConditionFlag.identity:
                        if (arr[tracker.id] < tracker.value)
                            arr[tracker.id] = tracker.value;
                        break;
                }
            }
            else
            {
                // Pack the message into a network variant, then unpack it and fire again as a TrackSuperlativeMessage
                tracker.id = clientIndex;
                EventController.FireEvent(new NetworkedSuperlativeMessage(tracker));
            }
        }
    }
}
