using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIKController : EventListener
{
    enum IKType
    {
        rightLeg,
        rightFoot,
        leftLeg,
        leftFoot,
    }

    public Transform[] raycastPositions;
    public IKSolver[] IK;
    public GameObject debugger;
    public bool showDebugger;
    private bool doIK = false;

    private void OnEnable()
    {
        EventController.AddListener(typeof(StopIKMessage), this);
        EventController.AddListener(typeof(StartIKMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(StopIKMessage), this);
        EventController.RemoveListener(typeof(StartIKMessage), this);
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if(doIK)
        {
            RaycastFoot();
        }
    }

    private void RaycastFoot()
    {
        RaycastHit hit;

        for (int i = 0; i < IK.Length; ++i)
        {
            Physics.Raycast(raycastPositions[i].position, Vector3.down, out hit, 10, ~(1 << 8));
            IK[i].SetTarget(hit.point + (Vector3.up * 0.2f));

            if (showDebugger)
            {
                GameObject n = Instantiate(debugger);
                n.transform.position = hit.point;
            }
        }
    }

    public void StopIK()
    {
        if (doIK)
        {
            doIK = false;
            foreach (IKSolver ik in IK)
            {
                ik.StopIK();
            }
        }
    }

    public void StartIK()
    {
        if (!doIK)
        {
            doIK = true;
            foreach (IKSolver ik in IK)
            {
                ik.StartIK();
            }
        }
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is StartIKMessage)
            StartIK();
        else if (e is StopIKMessage)
            StopIK();
    }

}
