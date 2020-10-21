using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class HoverOverAndClick : HoverOver, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnClick;

    public abstract void OnPointerDown(PointerEventData eventData);
    public abstract void OnPointerUp(PointerEventData eventData);
}
