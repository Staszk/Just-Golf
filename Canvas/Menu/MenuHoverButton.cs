using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuHoverButton : HoverOver
{
    [SerializeField] private Color[] colors = null;
    [SerializeField] private TMP_Text buttonText = null;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = colors[1];
        OnHoverOver.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = colors[0];
    }
}
