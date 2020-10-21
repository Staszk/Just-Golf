using UnityEngine;
using System.Collections.Generic;

public class RespectsFeedController : MonoBehaviour
{
    [SerializeField] private RespectsFeedElement prefab = null;
    [SerializeField] private Transform[] layers = null;
    private string[] names;
    [SerializeField] private Color[] playerColors = null;
    private Queue<RespectsFeedElement> queue;
    private List<RespectsFeedElement> activeMessages = null;

    private int id;

    public void Init()
    {
        activeMessages = new List<RespectsFeedElement>();

        names = new string[5] { "Orange", "Green", "Pink", "Blue", "You" };
        CreateElements();

        if (NetworkManager.instance)
            id = NetworkManager.instance.GetId();
    }

    // Update is called once per frame
    private void CreateElements()
    {
        queue = new Queue<RespectsFeedElement>();

        for (int i = 0; i < 10; i++)
        {
            RespectsFeedElement e = Instantiate(prefab, layers[0]);
            e.SetUp(this);
        }
    }

    public void Add(RespectsFeedElement e)
    {
        queue.Enqueue(e);
        e.transform.SetParent(layers[0]);
        activeMessages.Remove(e);
    }

    public void UseElement(int playerID)
    {
        if (playerID == id)
            return;

        if (queue.Count == 0)
        {
            activeMessages[0].Done();
        }

        RespectsFeedElement e = queue.Dequeue();
        e.transform.SetParent(layers[1]);
        e.transform.SetSiblingIndex(0);

        e.Set(names[playerID], playerColors[playerID]);
        activeMessages.Add(e);
    }

    public void Done()
    {
        while (activeMessages.Count > 0)
        {
            RespectsFeedElement e = activeMessages[0];
            e.Done();
        }
    }
}
