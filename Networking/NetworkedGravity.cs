using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedGravity : MonoBehaviour
{

    Rigidbody _rigid;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        bool _isTouchingFloor = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.15f, ~8);

        if (!_isTouchingFloor)
            _rigid.velocity += Vector3.up * Physics.gravity.y * 2.5f * Time.fixedDeltaTime;
        else
            _rigid.velocity = new Vector3(_rigid.velocity.x, 0, _rigid.velocity.z);
    }
}
