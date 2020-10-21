using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaveHealth
{
    void TakeDamage(float damage, int shooterID = -1);

    void SimulateDamage(Vector3 worldPos, float damage);

    float GetHealth();

    void GainHealth(float amount);
}
