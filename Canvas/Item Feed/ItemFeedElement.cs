using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFeedElement : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text playerName = null;
    [SerializeField] private UnityEngine.UI.Image icon = null;

    private ItemFeedController parent;
    public void SetUp(ItemFeedController ifc)
    {
        parent = ifc;
        Done();
    }

    public void Set(string name, Sprite icon, Color col)
    {
        this.icon.sprite = icon;
        playerName.text = name;
        playerName.color = col;
        gameObject.SetActive(true);
    }

    public void Done()
    {
        parent.Add(this);
        gameObject.SetActive(false);
    }
}
