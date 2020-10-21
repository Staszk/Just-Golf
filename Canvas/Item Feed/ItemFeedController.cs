using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFeedController : MonoBehaviour
{
    [SerializeField] private ItemFeedElement prefab = null;
    [SerializeField] private Transform[] layers = null;
    private string[] names;
    [SerializeField] private Color[] playerColors = null;
    private Queue<ItemFeedElement> queue;

    private int id;

    public void Init()
    {
        if (NetworkManager.instance)
            id = NetworkManager.instance.GetId();

        names = new string[5] { "Orange", "Green", "Pink", "Blue", "You" };
        CreateElements();
    }

    private void CreateElements()
    {
        queue = new Queue<ItemFeedElement>();

        for (int i = 0; i < 4; i++)
        {
            ItemFeedElement e = Instantiate(prefab, layers[0]);
            e.SetUp(this);
        }
    }

    public void Add(ItemFeedElement e)
    {
        queue.Enqueue(e);
        e.transform.SetParent(layers[0]);
    }

    public void UseElement(int userID, Sprite icon)
    {
        if (queue.Count == 0)
        {
            Debug.LogError("Queue empty");
            return;
        }

        ItemFeedElement e = queue.Dequeue();
        e.transform.SetParent(layers[1]);
        e.transform.SetSiblingIndex(0);

        string name = userID != id ? names[userID] : names[4];

        e.Set(name, icon, playerColors[userID]);
    }
}
