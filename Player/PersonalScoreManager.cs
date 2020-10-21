using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalScoreManager : EventListener
{
    public enum TYPE { 
        KILL_PLAYER, 
        SCORE_BALL
    }

    private int[] scores = new int[4];

    private void OnEnable()
    {
        EventController.AddListener(typeof(PersonalScoreMessage), this);
        EventController.AddListener(typeof(NetworkPersonalScoreMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(PersonalScoreMessage), this);
        EventController.RemoveListener(typeof(NetworkPersonalScoreMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is PersonalScoreMessage score)
        {
            HandleType(score.id, score.type);
        }else if (e is NetworkPersonalScoreMessage netScore)
        {
            HandleType(netScore.id, netScore.type);
        }
    }

    private void HandleType(int id, TYPE type)
    {
        if (type == TYPE.KILL_PLAYER) { scores[id] += 50; }
        else if (type == TYPE.SCORE_BALL) { scores[id] += 100; }
    }
}
