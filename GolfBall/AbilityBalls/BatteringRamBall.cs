using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamBall : AbilityBall
{
    private float yVectorIncrease = .5f;
    private float forceAmount = 8f;
    private float radius = 10f;
    private float velocityThreshold = 2.5f;
    public override void TriggerAbility()
    {
        //Ball Explosion
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in cols)
        {
            TargetDummy t = collider.GetComponent<TargetDummy>();

            if (t)
            {
                t.ShowHit(25);
            }

            //Send object away
            Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();

            if (rb)
            {
                //Get Directional vector away from ball
                Vector3 dir = (rb.gameObject.transform.position - gameObject.transform.position).normalized;

                //Add force in that direction + some Y component
                rb.AddForce(new Vector3(dir.x, yVectorIncrease, dir.z) * forceAmount, ForceMode.Impulse);
            }

        }
        base.TriggerAbility();
    }

    private void FixedUpdate()
    {
        Vector3 v = gameObject.GetComponent<Rigidbody>().velocity;

        if (v.x > velocityThreshold || v.x > velocityThreshold || v.x < -velocityThreshold || v.x < -velocityThreshold)
        {
            //Ball Explosion
            Collider[] cols = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider collider in cols)
            {
                if (collider != gameObject.GetComponent<Collider>() && collider != gameObject.GetComponentInChildren<Collider>())
                {
                    //Send object away
                    Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();

                    if (rb)
                    {
                        //Get Directional vector away from ball
                        Vector3 dir = (rb.gameObject.transform.position - gameObject.transform.position).normalized;

                        //Add force in that direction + some Y component
                        rb.AddForce(new Vector3(dir.x, yVectorIncrease, dir.z) * forceAmount, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
