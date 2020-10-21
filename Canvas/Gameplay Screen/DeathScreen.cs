using UnityEngine;
using System.Collections;
using TMPro;

public sealed class DeathScreen : MonoBehaviour
{
    [SerializeField] TMP_Text playerText = null;
    [SerializeField] Color[] playerColors = null;
    [SerializeField] TMP_Text respawnTimer = null;

    private readonly string[] names = { "Orange", "Green", "Pink", "Blue" };

    public void NewDeath(int ID)
    {
        playerText.text = names[ID];
        playerText.color = playerColors[ID];

        gameObject.SetActive(true);
    }

    public void UpdateTimer(float time)
    {
        respawnTimer.text = string.Format("{0:0.00}", time) + "s";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
