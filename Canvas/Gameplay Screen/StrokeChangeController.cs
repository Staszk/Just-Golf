using UnityEngine;
using System.Collections.Generic;
using System;

public class StrokeChangeController : MonoBehaviour
{
    [SerializeField] private StrokeChangeUI prefab = null;
    [SerializeField] private Transform[] layers = null;

    private Queue<StrokeChangeUI> queue;

    public void Init()
    {
        CreateUIObjects();
    }

    private void CreateUIObjects()
    {
        queue = new Queue<StrokeChangeUI>();

        for (int i = 0; i < 10; i++)
        {
            StrokeChangeUI ui = Instantiate(prefab, layers[1]);
            ui.SetUp(this);
        }
    }

    public void Add(StrokeChangeUI ui)
    {
        queue.Enqueue(ui);
        ui.transform.SetParent(layers[1]);
    }

    public void UseObject(string s, Color c)
    {
        StrokeChangeUI ui = queue.Dequeue();
        ui.transform.SetParent(layers[0]);
        ui.Set(s, c);
    }
}
