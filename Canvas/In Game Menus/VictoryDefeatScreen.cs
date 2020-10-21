using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryDefeatScreen : EventListener
{
    [SerializeField] private GameObject victoryUI = null;
    [SerializeField] private GameObject defeatUI = null;

    //If the victory/defeat screen is showing
    private bool onScreen = false;

    private void OnEnable()
    {
        EventController.AddListener(typeof(VictoryDefeatMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(VictoryDefeatMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is VictoryDefeatMessage vd)
        {
            if (vd.isVictorious)
                ToggleVictoryScreen(true);
            else
                ToggleShowDefeatScreen(true);

            onScreen = true;
        }
    }

    private void Update()
    {
        if(onScreen)
        {
            if (KeybindingController.GetInput(GameControls.Jump))
            {
                onScreen = false;
                ToggleVictoryScreen(false);
                ToggleShowDefeatScreen(false);
                EventController.FireEvent(new ShowEndScreenMessage());
            }
        }
    }

    private void ToggleVictoryScreen(bool b)
    {
        victoryUI.SetActive(b);
    }

    private void ToggleShowDefeatScreen(bool b)
    {
        defeatUI.SetActive(b);
    }
}
