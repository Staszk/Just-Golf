using UnityEngine;
using System.Collections;

public class EliminatedFeedElement : MonoBehaviour
{
    private UnityEngine.UI.Image img = null;

    private EliminatedFeedController parent;

    public void Setup(EliminatedFeedController efc)
    {
        parent = efc;
        img = GetComponent<UnityEngine.UI.Image>();
        Done();
    }

    public void Set(Sprite spr)
    {
        img.sprite = spr;
        gameObject.SetActive(true);
    }

    public void Done()
    {
        parent.Add(this);
        gameObject.SetActive(false);
    }
}
