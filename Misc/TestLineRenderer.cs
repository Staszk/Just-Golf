//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[ExecuteInEditMode]
//public class TestLineRenderer : MonoBehaviour
//{
//    [SerializeField] private ClubStat[] clubStats = null;
//    public float distance = 10;
//    public float adder = 0.5f;

//    public Vector3 direction;
//    public float power;

//    LineRenderer line;


//    // Start is called before the first frame update
//    void OnEnable()
//    {
//        line = GetComponent<LineRenderer>();
//        Vector3 velocity = direction * power;
//        line.positionCount = 0;
//        int counter = 0;
//        for (float i = 0; i < distance; i += adder)
//        {
//            line.positionCount++;
//            CalculatePoint(velocity, i, counter);
//            counter++;
//        }

//    }

//    // Update is called once per frame
//    void Update()
//    {
       
//    }

//    Vector3 CalculatePoint(Vector3 velocity, float time, int point)
//    {
//        Vector3 pos = (direction * power) * time + 0.5f * Physics.gravity * time * time;
//        line.SetPosition(point, pos);
//        return velocity + Physics.gravity * time;

//    }
//}
