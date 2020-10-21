using UnityEngine;
using System.Collections;

public class RespectsFeedElement : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text playerText = null;

    private RespectsFeedController parent;

    public void SetUp(RespectsFeedController rfc)
    {
        parent = rfc;
        Done();
    }

    public void Set(string player, Color playerColor)
    {
        playerText.text = player;
        playerText.color = playerColor;
        gameObject.SetActive(true);
    }

    public void Done()
    {
        parent.Add(this);
        gameObject.SetActive(false);
    }
}
