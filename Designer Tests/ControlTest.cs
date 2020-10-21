using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTest : MonoBehaviour
{
    float sped = 60f;
    bool inGolf = false;
    float forward = 0f;
    float hori = 0f;
    float friction = .8f;
    public GameObject ball;
    public GameObject guideline;
    public GameObject player;
    Rigidbody rb;
    Vector3 playerStart;

    float timer = 5f;
    bool timerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerStart = player.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        forward = Input.GetAxis("Vertical") * sped;
        hori = Input.GetAxis("Horizontal") * sped;

        if (!inGolf)
        {
            rb.AddForce(transform.forward * forward);
            rb.AddForce(transform.right * hori);
            //Artificial Friction
            rb.velocity = new Vector3(rb.velocity.x * friction, rb.velocity.y, rb.velocity.z * friction);
        }

        bool insideBall = InsideBall();

        if (inGolf && Input.GetKeyDown(KeyCode.Q) && !timerOn)
        {
            ExitGolf();
            timerOn = true;
        }


        if (Input.GetKeyDown(KeyCode.Q) && insideBall && !timerOn)
        {
            Golf();
            timerOn = true;
        }

        if (inGolf)
        {
            Golf();
        }

        

        if (Input.GetMouseButtonDown(0) && inGolf)
        {
            FuckingYEET();
        }

        if (timerOn)
        {
            timer--;
            if (timer <= 0)
            {
                timerOn = false;
                timer = 10;
            }
        }


    }

    private void ExitGolf()
    {
        inGolf = false;
        guideline.SetActive(false);
        player.transform.Rotate(0, -90, 0);
        player.transform.position = transform.position;
    }

    bool InsideBall()
    {
        if (Vector3.Distance(transform.position, ball.transform.position) < 2)
        {
            return true;
        }

        return false;
    }

    

    private void Golf()
    {
        rb.velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //Reposition fucker
        if (!inGolf)
        {
            playerStart = player.transform.localPosition;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = new Vector3(ball.transform.position.x, transform.position.y, ball.transform.position.z);
            ball.transform.rotation = transform.rotation;
            guideline.SetActive(true);
            player.transform.position = new Vector3(player.transform.position.x - 1, player.transform.position.y, player.transform.position.z);
            player.transform.Rotate(0, 90, 0);
            inGolf = true;
        }

        //Rotate ball
        ball.transform.rotation = transform.rotation;
    }

    private void FuckingYEET()
    {
        ExitGolf();
        ball.GetComponent<Rigidbody>().AddForce(ball.transform.forward * 800);
    }
}
