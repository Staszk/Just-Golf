using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GolfBall : MonoBehaviour
{
    protected GolfBallManager parent;
    protected bool hit;

    protected int IDofLastHit;

    protected Rigidbody rgbd;

    // Misc.
    private readonly float radiusToGround = 0.16f;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] protected float damage = 25;

    protected bool isAbilityBall = false;
    protected NetworkManager network;

    public virtual void Initialize(GolfBallManager gbm)
    {
        parent = gbm;

        rgbd = GetComponent<Rigidbody>();
        network = NetworkManager.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hit)
        {
            EventController.FireEvent(new WorldVFXMessage(WorldSpaceVFX.WorldVFXType.impact, transform.position));
            if(network && network.IsHost()) { network.SendWorldVFX(WorldSpaceVFX.WorldVFXType.impact, transform.position); }

            //For Sounds when colliding with objects
            if (collision.gameObject.tag == "MetalSound")
            {
                SoundManager.PlaySoundAt("Metal Hit", transform.position);
            }

            if (collision.gameObject.tag == "WoodSound")
            {
                SoundManager.PlaySoundAt("Wood Hit", transform.position);
            }

            if (collision.gameObject.tag == "RockSound")
            {
                SoundManager.PlaySoundAt("Rock Hit", transform.position);
            }

            //Hole Detection
            if (collision.gameObject.tag == "Hole")
            {
                collision.gameObject.GetComponent<Hole>().CheckBallScore(this);
            }
        }
    }

    public void Prepare()
    {
        hit = false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, radiusToGround, groundLayer); 
    }

    public bool IsStationary()
    {
        return rgbd.velocity.sqrMagnitude < 0.02f;
    }

    public float GetDamage() { return damage; }
    public int GetLastShotId() { return IDofLastHit; }
    public bool IsAbilityBall() { return isAbilityBall; }

    public Vector3 GetVelocity() { return rgbd.velocity; }

    public void HitBall(int playerID, bool isLocal)
    {
        if (!parent)
            return;

        IDofLastHit = playerID;

        if (!hit & isLocal)
        {
            hit = true;
        }
    }

    public void EndLifetime()
    {
        parent.RespawnBall(this);
    }
}
