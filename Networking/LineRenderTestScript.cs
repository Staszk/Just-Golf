using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderTestScript : MonoBehaviour
{
    private LineRenderer line;
    private float distance = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        DoLine();
    }

    // Update is called once per frame
    void DoLine()
    {
        float currentDist = 0;
        line.positionCount = 1;
        line.SetPosition(0, transform.position);
        Vector3 dir = Vector3.right;
        RaycastHit hit;
        int index = 1;
        while(currentDist < distance && index < 50)
        {
            line.positionCount++;
            Vector3 p0 = transform.TransformPoint(line.GetPosition(index - 1));
            bool hashit = Physics.Raycast(p0, dir, out hit, Mathf.Infinity);
            if (hashit)
            {
                print(index);
                line.SetPosition(index, hit.point);
                currentDist = distance;
            }
            else
            {
                print("hmm");
            }

            index++;
        }
    }

}
