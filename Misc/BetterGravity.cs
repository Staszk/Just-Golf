using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterGravity : MonoBehaviour
{
    public float _gravityFactor = 2.5f;

    private Vector3 _gravityConstant;
    private Rigidbody _rigid;
    private PlayerScoreTracker localPlayerScoreTracker;

    public bool ShouldUpdateGravity { get; set; } = true;

    public Vector3 GetGravityConstant() { return _gravityConstant; }
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        //localPlayerScoreTracker = GameObject.Find("Player").GetComponent<PlayerScoreTracker>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if(ShouldUpdateGravity)
          _rigid.velocity += (Vector3.up * Physics.gravity.y * _gravityFactor * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Hole")
        {
            if (NetworkManager.instance)
                NetworkManager.instance.SendGoalUpdate(gameObject);
            else
                localPlayerScoreTracker.ConvertPoints();    
        }
    }
}
