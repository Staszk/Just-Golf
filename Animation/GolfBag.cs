using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBag : MonoBehaviour
{
    public Transform jointToFollow;
    public float forwardDistance;
    public float upDistance;
    public float rightDistance;
    public Vector3 rotation;

    void Update()
    {
        transform.position = jointToFollow.position + (transform.forward * forwardDistance) + (transform.up * upDistance) + (transform.right * rightDistance);
        transform.eulerAngles = new Vector3(jointToFollow.eulerAngles.x + rotation.x, 
            jointToFollow.eulerAngles.y + rotation.y, 
            jointToFollow.eulerAngles.z + rotation.z);
    }
}
