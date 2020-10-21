using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUpManager : MonoBehaviour
{

    [SerializeField] private GameObject[] healthBoxes = null;

    private void OnEnable()
    {
        NetworkItemManager.EventHealthPickUp += disableBox;
    }

    private void OnDisable()
    {
        NetworkItemManager.EventHealthPickUp -= disableBox;
    }


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < healthBoxes.Length; i++)
        {
            healthBoxes[i].GetComponent<HealthPickUp>().SetID(i);
        }
    }

    private void disableBox(int id)
    {
        //print()
        healthBoxes[id].GetComponent<HealthPickUp>().TurnOffPickUp();
    }

}
