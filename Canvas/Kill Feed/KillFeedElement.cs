using UnityEngine;
using System.Collections;

public class KillFeedElement : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text killerText = null;
    [SerializeField] private TMPro.TMP_Text killedText = null;

    private KillFeedController parent;

    public void SetUp(KillFeedController kfc)
    {
        parent = kfc;
        Done();
    }

    public void Set(string killer, string killed, Color killerColor, Color killedColor)
    {
        killerText.text = killer;
        killerText.color = killerColor;
        killedText.text = killed;
        killedText.color = killedColor;
        gameObject.SetActive(true);
    }

    public void Done()
    {
        parent.Add(this);
        gameObject.SetActive(false);
    }
}
