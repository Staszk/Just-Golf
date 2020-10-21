using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlaceLetterScript : MonoBehaviour
{
    public TMP_Text placeText;
    public TMP_Text placeLetterText;

    // Update is called once per frame
    void Update()
    {
        UpdatePlaceText();
    }

    private void UpdatePlaceText()
    {
        if (placeText.text == "1")
        {
            placeLetterText.text = "st";
        }

        if (placeText.text == "2")
        {
            placeLetterText.text = "nd";
        }

        if (placeText.text == "3")
        {
            placeLetterText.text = "rd";
        }

        if (placeText.text == "4")
        {
            placeLetterText.text = "th";
        }
    }
}
