using UnityEngine;
using System.Collections;

public class Winner : EventListener
{
    [SerializeField] private GameObject crown = null;
    private bool isInTheLead = false;

    private void OnEnable()
    {
        EventController.AddListener(typeof(ClientDeathMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ClientDeathMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ClientDeathMessage)
        {
            DeactivateCrown();
        }
        else if (e is ClientRespawnMessage)
        {
            ActivateCrown();
        }
    }

    public void BecomeWinner(bool isWinner)
    {
        crown.SetActive(isWinner);
        isInTheLead = isWinner;
    }

    private void DeactivateCrown()
    {
        crown.SetActive(false);
    }

    private void ActivateCrown()
    {
        if (isInTheLead) { crown.SetActive(true); }      
    }
}
