using UnityEngine;
using System.Collections.Generic;

public class KillFeedController : MonoBehaviour
{
    [SerializeField] private KillFeedElement prefab = null;
    [SerializeField] private Transform[] layers = null;
    private string[] names;
    [SerializeField] private Color[] playerColors = null;
    private Queue<KillFeedElement> queue;

    private int id;

    public void Init()
    {
        names = new string[5] { "Orange", "Green", "Pink", "Blue", "You" };
        CreateElements();

        if (NetworkManager.instance)
            id = NetworkManager.instance.GetId();
    }

    private void CreateElements()
    {
        queue = new Queue<KillFeedElement>();

        for (int i = 0; i < 4; i++)
        {
            KillFeedElement e = Instantiate(prefab, layers[0]);
            e.SetUp(this);
        }
    }

    public void Add(KillFeedElement e)
    {
        queue.Enqueue(e);
        e.transform.SetParent(layers[0]);
    }

    public void UseElement(int killerID, int killedID)
    {
        if (queue.Count == 0)
        {
            Debug.LogError("Queue empty");
            return;
        }

        KillFeedElement e = queue.Dequeue();
        e.transform.SetParent(layers[1]);
        e.transform.SetSiblingIndex(0);

        string name = killerID != id ? names[killerID] : names[4];

        e.Set(name, names[killedID], playerColors[killerID], playerColors[killedID]);
    }
}
