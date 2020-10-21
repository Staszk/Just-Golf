using UnityEngine;
using System.Collections.Generic;

public class EliminatedFeedController : MonoBehaviour
{
    [SerializeField] private EliminatedFeedElement prefab = null;
    [SerializeField] private Transform[] layers = null;
    [SerializeField] private Sprite[] sprites = null;
    private Queue<EliminatedFeedElement> queue;
    private int id;

    public void Init()
    {
        CreateElements();

        if (NetworkManager.instance)
            id = NetworkManager.instance.GetId();
    }

    private void CreateElements()
    {
        queue = new Queue<EliminatedFeedElement>();

        for (int i = 0; i < 2; i++)
        {
            EliminatedFeedElement e = Instantiate(prefab, layers[0]);
            e.Setup(this);
        }
    }

    public void Add(EliminatedFeedElement e)
    {
        queue.Enqueue(e);
        e.transform.SetParent(layers[0]);
    }

    public void UseElement(int killerID, int killedID)
    {
        if (killerID == id)
        {
            if (queue.Count == 0)
            {
                Debug.LogError("Queue empty");
                return;
            }

            EliminatedFeedElement e = queue.Dequeue();
            e.transform.SetParent(layers[1]);
            e.transform.SetSiblingIndex(0);
            e.Set(sprites[killedID]);
        }
    }
}
