using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinButtonScript : MonoBehaviour
{
    public GameObject hostButton;
    public GameObject joinButton;
    public GameObject ipEnterText;
    public GameObject instructionsText;

    public void ButtonHit()
    {
        hostButton.SetActive(false);
        joinButton.SetActive(false);
        ipEnterText.SetActive(true);
        instructionsText.SetActive(true);
    }
}
