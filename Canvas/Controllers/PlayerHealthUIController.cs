using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthUIController : EventListener
{
    [SerializeField] private HealthBarUI playerHealthBar = null;
    [SerializeField] private HealthBarUI teammateHealthBar = null;

    private void Start()
    {
        playerHealthBar.ShowShield(false);
        teammateHealthBar.ShowShield(false);
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(PlayerHealthChangeMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(PlayerHealthChangeMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is PlayerHealthChangeMessage damageMessage)
        {
            playerHealthBar.DisplayHealth(damageMessage.currentHealth);
        }
    }

    public void ChangeToAwayTeam()
    {
        playerHealthBar.ChangeToAwayTeam();
        teammateHealthBar.ChangeToAwayTeam();
    }
}
