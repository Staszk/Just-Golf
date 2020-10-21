using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    float height = 2f;
    float distance = 7f;
    float rotDamping = 5f;


    // Update is called once per frame
    void LateUpdate()
    {
        float wantedRotationAngle = target.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;
        //LERP the rotation
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotDamping * Time.deltaTime);
        //LERP the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, Time.deltaTime);
        //Get the rotation
        Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
        //position the camera
        transform.position = target.transform.position;
        //set its offset distance
        transform.position -= currentRotation * Vector3.forward * distance;
        //set its offset height
        Vector3 newHeight = transform.position;
        newHeight.y = currentHeight;
        transform.position = newHeight;
        transform.LookAt(target);

    }
}
