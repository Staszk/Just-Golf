using UnityEngine;
using System.Collections;

public abstract class EventListener : MonoBehaviour
{
    public abstract void HandleEvent(EventMessage e);
}
