using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Subscriptions
{
    public List<EventListener> priorityListeners;
    public List<EventListener> normalListeners;
}

public static class EventController
{
    private static Dictionary<System.Type, Subscriptions> pairs;
    private static bool initialized = false;
 
    private static void Initialize()
    {
        if (initialized)
            return;

        initialized = true;
        pairs = new Dictionary<System.Type, Subscriptions>();
    }

    public static void AddListener(System.Type e, EventListener listener, bool isPriority = false)
    {
        if (!initialized)
            Initialize();

        if (pairs.ContainsKey(e))
        {
            List<EventListener> l = !isPriority ? pairs[e].normalListeners : pairs[e].priorityListeners;
            l.Add(listener);
        }
        else
        {
            Subscriptions newSubscriptions;

            newSubscriptions.normalListeners = new List<EventListener>();
            newSubscriptions.priorityListeners = new List<EventListener>();

            List<EventListener> l = !isPriority ? newSubscriptions.normalListeners : newSubscriptions.priorityListeners;
            l.Add(listener);

            pairs.Add(e, newSubscriptions);
        }
    }

    public static void RemoveListener(System.Type e, EventListener listener, bool isPriority = false)
    {
        if (pairs.ContainsKey(e))
        {
            List<EventListener> l = !isPriority ? pairs[e].normalListeners : pairs[e].priorityListeners;
            l.Remove(listener);
        }
    }

    public static void FireEvent(EventMessage eventMessage)
    {
        System.Type t = eventMessage.GetType();

        if (pairs.ContainsKey(t))
        {
            foreach (EventListener listener in pairs[t].priorityListeners)
            {
                listener.HandleEvent(eventMessage);
            }

            foreach (EventListener listener in pairs[t].normalListeners)
            {
                listener.HandleEvent(eventMessage);
            }
        }
        else
        {
            Debug.LogWarning("No listeners for " + t.ToString());
        }
    }
}
