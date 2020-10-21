using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Honing : MonoBehaviour
{
    [SerializeField] private float strength = 0.5f;
    [Range(0,0.25f)]
    [SerializeField] private float trackingRadius = 0.05f;
    [SerializeField] private float homingRadius = 10;


    private GolfBall myGolfBall;
    private Rigidbody rb;

    NetworkStoredData nStoredData;


    void Start()
    {
        myGolfBall = GetComponent<GolfBall>();
        rb = GetComponent<Rigidbody>();
        nStoredData = NetworkStoredData.instance;
    }

    private GameObject GetClosestPlayer(Collider[] players)
    {
        int index = -1; 
        float distance = float.MaxValue;
        Vector3 direction;
        float squareMag = -1;
        for (int i = 0; i < players.Length; i++)
        {
            if (nStoredData)
            {
                EntityHealth target = players[i].GetComponent<EntityHealth>();
                if (target != null)
                {
                    int playerId = target.MyID;
                    if (nStoredData.GetTeam(playerId) == nStoredData.GetTeam(myGolfBall.GetLastShotId())) { continue; }
                }                
            }

            direction = players[i].gameObject.transform.position - transform.position;
            if (Vector3.Dot(rb.velocity.normalized, direction.normalized) > 0) //Is in screen view 
            {                
                squareMag = Vector3.SqrMagnitude(direction);
                if (squareMag < distance)
                {
                    index = i;
                    distance = squareMag;
                }
            }
        }

        if (index >= 0) { return players[index].gameObject; }
        else { return null;}
    } 

    public int GetIDOfClosestPlayer()
    {
        Camera cam = Camera.main;
        GameObject closestPlayer = null;
        float dot = -2;

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Networked Player");

        foreach(GameObject p in allPlayers)
        {
            Vector3 localPoint = cam.transform.InverseTransformPoint(p.transform.position).normalized;
            float dotCheck = Vector3.Dot(localPoint, Vector3.forward);

            Vector3 screenPos = cam.WorldToViewportPoint(p.transform.position);
            if (screenPos.x > (0.5f + trackingRadius) || screenPos.x < (0.5f - trackingRadius))
                continue;

            if(dotCheck > dot)
            {
                dot = dotCheck;
                closestPlayer = p;
            }
        }

        if (closestPlayer != null)
        {
            return int.Parse(closestPlayer.name);
        }
        
        return -1;
    }
    public GameObject GetObjOfClosestPlayer()
    {
        Camera cam = Camera.main;
        GameObject closestPlayer = null;
        float dot = -2;

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Networked Player");

        foreach (GameObject p in allPlayers)
        {
            Vector3 localPoint = cam.transform.InverseTransformPoint(p.transform.position).normalized;
            float dotCheck = Vector3.Dot(localPoint, Vector3.forward);

            Vector3 screenPos = cam.WorldToViewportPoint(p.transform.position);
            if (screenPos.x > (0.5f + trackingRadius) || screenPos.x < (0.5f - trackingRadius))
                continue;

            if (dotCheck > dot)
            {
                dot = dotCheck;
                closestPlayer = p;
            }
        }
        return closestPlayer;
    }

    public void HoneToPlayer(GameObject obj)
    {
        StopAllCoroutines();
        StartCoroutine(Track(obj));
    }
  
    IEnumerator Track(GameObject player)
    {
        yield return new WaitForFixedUpdate();

        //Stay in loop of checking for player within the homing radius until the ball hits the ground
        while (!myGolfBall.IsGrounded())
        {
            yield return new WaitForEndOfFrame();

            while (Vector3.Distance(player.transform.position, transform.position) <= homingRadius)
            {
                if (myGolfBall.IsGrounded()) { break; }

                Vector3 initialVel = rb.velocity;
                float speed = initialVel.magnitude;
                Vector3 direction = ((player.transform.position + Vector3.up * 2) - gameObject.transform.position);
                Vector3 currentVelocity = direction.normalized * speed;
                currentVelocity = Vector3.Lerp(rb.velocity, currentVelocity, strength).normalized * speed;
                rb.velocity = currentVelocity;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
