using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscTestScript : MonoBehaviour
{
    public float speed;

    public GameObject[] planes;
    private int index = 0;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.P))
        {
            planes[index].SetActive(false);

            int newIndex = (index + 1) % 3;

            planes[newIndex].SetActive(true);
            index = newIndex;
        }
    }
}
