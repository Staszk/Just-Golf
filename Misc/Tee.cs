using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tee : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 0f;
    private float currentLifetime;

    private void OnEnable()
    {
        currentLifetime = 0;
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;

        if (currentLifetime >= maxLifetime)
        {
            TeeManager.ResetTee(this);
        }
    }
}
