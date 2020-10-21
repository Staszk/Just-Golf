using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HealthPickUp : MonoBehaviour
{
    private readonly float healthGainAmount = 100f;
    float respawnTime = 15.0f;
    private int healthId = -1;

    public void SetID(int id) { healthId = id; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Networked Player"))
        {
            // Play the Sound 
            
            if (other.GetComponent<IHaveHealth>().GetHealth() >= 100)
            {
                if (other.CompareTag("Player"))
                {
                    SoundManager.PlaySound("Full Health");
                    EventController.FireEvent(new AtFullHealthMessage());
                }
                return;
            }

            if (!NetworkManager.instance)
            {
                other.GetComponent<PlayerHealth>().GainHealth(healthGainAmount);
                SoundManager.PlaySound("Health Box Heal");
                TurnOffPickUp();
            }               
            else
            {
                if (NetworkManager.instance.IsHost())
                {
                    NetworkManager.instance.SendHealRequest(healthGainAmount, other.gameObject, healthId);
                }
            }          
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void TurnOffPickUp()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        SoundManager.PlaySoundAt("Health Box Break", gameObject.transform.position);
        StartCoroutine(Respawn());
    }
}
