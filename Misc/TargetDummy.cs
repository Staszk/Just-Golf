using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public bool move;
    public float speed;
    public Transform[] moveNodes;
    public Material hurtMat;
    private Material originalMat;
    private int nodeIndex = 0;
    private int health = 100;
    private bool shielded = false;

    private Coroutine damageCoroutine = null;
    private Coroutine freezeCoroutine = null;

    private void Start()
    {
        originalMat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if(move && moveNodes.Length > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveNodes[nodeIndex].position, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, moveNodes[nodeIndex].position) < 1)
            {
                nodeIndex++;
                if (nodeIndex == moveNodes.Length)
                    nodeIndex = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("GolfBall"))
        {
            //Determine type of ball
            if (collision.gameObject.GetComponentInParent<BombAbility>())
            {
                //Ability should trigger in BombAbility Script. This acts as a catch to prevent repeating damage
            }
            else if (collision.gameObject.GetComponentInParent<IceBall>())
            {
                //Ability should trigger in IceBall Script.
            }
            else if (collision.gameObject.GetComponentInParent<ShieldBall>())
            {
                //Ability should be triggered in ShieldBall Script
            }
            else if (collision.gameObject.GetComponentInParent<BatteringRamBall>())
            {
                //Ability should be triggered in BatteringRamBallScript
            }
            else
            {
                //Regular ball & storage ball damage
                ShowHit(25);
            }
            
        }
    }

    public void ShowHit(int damage)
    {
        //Deal damage appropriately
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        if (shielded)
        {
            TurnOffShield();
        }
        else
        {
            health -= damage;
            damageCoroutine = StartCoroutine(ShowPain());
        }
    }

    private IEnumerator ShowPain()
    {
        GetComponent<MeshRenderer>().material = hurtMat;
        yield return new WaitForSeconds(0.25f);
        GetComponent<MeshRenderer>().material = originalMat;

        //Check if dummy died
        if (health <= 0)
        {
            health = 0;
            gameObject.SetActive(false);
        }

    }

    public void FreezeDummy(float duration)
    {
        if (freezeCoroutine != null)
            StopCoroutine(freezeCoroutine);

        freezeCoroutine = StartCoroutine(Freeze(duration));
    }

    private IEnumerator Freeze(float duration)
    {
        GetComponent<MeshRenderer>().material = hurtMat;
        move = false;
        yield return new WaitForSeconds(.25f);
        GetComponent<MeshRenderer>().material = originalMat;
        yield return new WaitForSeconds(duration - .25f);
        move = true;
    }

    public void TurnOnShield()
    {
        shielded = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void TurnOffShield()
    {
        shielded = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
