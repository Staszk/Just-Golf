using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRetrieval : MonoBehaviour
{
    public bool resetAtOutOfBounds = true;
    private Vector3 lastPos;
    private NetworkManager network;
    // Start is called before the first frame update
    void Start()
    {
        SetNewBallPos();
        network = NetworkManager.instance;
    }

    public void SetNewBallPos()
    {
        lastPos = gameObject.transform.position;
    }

    public Vector3 GetLastPos()
    {
        return lastPos;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "OutOfBounds")
    //    {
    //        if (!network || network.GetId() == 0)
    //        {
    //            gameObject.transform.position = lastPos;
    //            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //        }
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "OutOfBounds" && resetAtOutOfBounds)
        {
            if (!network || network.GetId() == 0)
            {
                gameObject.transform.position = lastPos;
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.BreakingTheGame, SuperlativeController.ConditionFlag.additive, 1));
            }
        }
    }
}
