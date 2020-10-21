using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadEntity : MonoBehaviour, IHaveHealth
{
    private Transform parent;
    private IHaveHealth parentHealth;

    private readonly float headshotMultiplier = 2.0f;

    public void SetUp(HeadPlacement h, Transform p, IHaveHealth ph)
    {
        transform.position = p.transform.position + h.Offset;
        transform.localScale = h.ScaleValues;
        transform.SetParent(p);

        gameObject.AddComponent<SphereCollider>();

        parent = p;
        parentHealth = ph;
    }

    public void TakeDamage(float damage, int shooterID = -1)
    {       
        Debug.LogWarning("headshot");
        parentHealth.TakeDamage(damage * headshotMultiplier, shooterID);
    }

    public void SimulateDamage(Vector3 worldPos, float damage)
    {
        GameplayUIController.instance.UsePopOff(new SimulatedDamageMessage(worldPos, damage * headshotMultiplier, true));
    }

    public float GetHealth() { return parentHealth.GetHealth(); }

    public void GainHealth(float amount)
    {
        parentHealth.GainHealth(amount);
    }
}
