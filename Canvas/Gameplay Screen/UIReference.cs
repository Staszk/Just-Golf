using TMPro;
using UnityEngine;

public class UIReference : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText = null;

    public void SetDistanceText(int distance)
    {
        if (distanceText != null)
        {
            distanceText.text = distance.ToString() + "m";
        }
    }
}
