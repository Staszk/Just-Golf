using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    public float xRotation;
    public float yRotation;
    public float lookSensitivity = 5f;

    public float currentXRotation;
    public float currentYRotation;
    public float lookSmoothDamp = 1.0f;
    float xRotV;
    float yRotV;

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    void Look()
    {
        //xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;

        //xRotation = Mathf.Clamp(xRotation, -75, 75);

        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotV, lookSmoothDamp);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotV, lookSmoothDamp);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
    }
}
