using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseSensitivitySlider : MonoBehaviour
{
    [SerializeField] private int mouseSensitivityIndex = 0;
    [SerializeField] private TMP_Text valueText = null;

    private void Start()
    {
        valueText.text = GetComponent<UnityEngine.UI.Slider>().value.ToString();
    }

    public void ValueChanged(float value)
    {
        ClientSettings.UpdateSensitivity(mouseSensitivityIndex, value);
        valueText.text = value.ToString();
    }
}
