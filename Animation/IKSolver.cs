using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    struct IKJoint
    {
        public Transform transform;
        public Vector3 position;
        public Vector3 startDirection;
        public Quaternion startRotation;
    }

    public int boneAmount;
    public Transform constraint;
    public int iterations;
    public Transform player;
    Vector3 target;
    float[] bonesLength;
    float totalLength;
    IKJoint[] ikJoints;
    bool doIK = false;

    public bool rotFix;

    void Awake()
    {
        Init();
    }

    void LateUpdate()
    {
        if(doIK)
            ResolveIK();
    }

    private void Init()
    {
        ikJoints = new IKJoint[boneAmount + 1];
        bonesLength = new float[boneAmount];
        totalLength = 0;
        Transform current = transform;
        for (int i = ikJoints.Length - 1; i >= 0; i--)
        {
            ikJoints[i].transform = current;
            ikJoints[i].startRotation = current.rotation;
            if (i != ikJoints.Length - 1)
            {
                ikJoints[i].startDirection = ikJoints[i + 1].transform.position - current.position;
                bonesLength[i] = ikJoints[i].startDirection.magnitude;
                totalLength += bonesLength[i];
            }
            else
                ikJoints[i].startDirection = player.forward;
            
            current = current.parent;
        }
    }
    void ResolveIK()
    {
        if (target == null)
            return;

        for (int i = 0; i < ikJoints.Length; i++)
            ikJoints[i].position = ikJoints[i].transform.position;

        if ((target - ikJoints[0].transform.position).sqrMagnitude >= totalLength * totalLength)
        {
            Vector3 direction = (target - ikJoints[0].position).normalized;
            for (int i = 1; i < ikJoints.Length; i++)
                ikJoints[i].position = ikJoints[i - 1].position + direction * bonesLength[i - 1];
        }
        else
        {
            for (int i = 0; i < iterations; i++)
            {
                ikJoints[ikJoints.Length - 1].position = target;
                for (int j = ikJoints.Length - 2; j > 0; j--)
                    ikJoints[j].position = ikJoints[j + 1].position + (ikJoints[j].position - ikJoints[j + 1].position).normalized * bonesLength[j];

                //for (int j = 1; j < ikJoints.Length; j++)
                //    ikJoints[j].position = ikJoints[j - 1].position + (ikJoints[j].position - ikJoints[j - 1].position).normalized * bonesLength[j - 1];

                if ((target - ikJoints[ikJoints.Length - 1].transform.position).sqrMagnitude < 0.01f * 0.01f)
                    break;
            }
        }

        for (int i = 1; i < ikJoints.Length - 1; i++)
        {
            Plane plane = new Plane(ikJoints[i + 1].position - ikJoints[i - 1].position, ikJoints[i - 1].position);
            Vector3 projectedPole = plane.ClosestPointOnPlane(constraint.position);
            Vector3 projectedBone = plane.ClosestPointOnPlane(ikJoints[i].position);
            float angle = Vector3.SignedAngle(projectedBone - ikJoints[i - 1].position, projectedPole - ikJoints[i - 1].position, plane.normal);
            ikJoints[i].position = Quaternion.AngleAxis(angle, plane.normal) * (ikJoints[i].position - ikJoints[i - 1].position) + ikJoints[i - 1].position;
        }

        for (int i = 0; i < ikJoints.Length - 1; i++)
        {
            ikJoints[i].transform.rotation = Quaternion.FromToRotation(ikJoints[i].startDirection, 
                ikJoints[i + 1].position - ikJoints[i].position) * ikJoints[i].startRotation * Quaternion.LookRotation(-player.forward, player.up);

            ikJoints[i].transform.position = ikJoints[i].position;
        }
    }

    public void SetTarget(Vector3 pos)
    {
        target = pos;
        doIK = true;
    }

    public void StopIK()
    {
        doIK = false;
    }

    public void StartIK()
    {
        doIK = true;
    }
}
