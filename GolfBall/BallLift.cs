using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLift : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public IEnumerator AddLift(float power)
    {
        rb.useGravity = false;
        yield return new WaitForSeconds(power * 0.0025f);
        rb.useGravity = true;

    }
}
