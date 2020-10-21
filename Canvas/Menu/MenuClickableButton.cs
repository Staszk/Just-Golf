using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuClickableButton : HoverOverAndClick
{
    [SerializeField] private Color[] textColors = null;
    [SerializeField] private TMP_Text buttonText = null;
    [SerializeField] private UnityEngine.UI.Image image = null;

    public void ResetColor()
    {
        buttonText.color = textColors[0];
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (buttonText)
            buttonText.color = textColors[2];

        if (image)
            image.color = textColors[2];

        SoundManager.PlaySound("Button Press");
        OnClick.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverOver.Invoke();

        if (buttonText)
            buttonText.color = textColors[1];

        if (image)
            image.color = textColors[1];

        isHovering = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText)
            buttonText.color = textColors[0];

        if (image)
            image.color = textColors[0];

        isHovering = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (buttonText)
            buttonText.color = isHovering ? textColors[1] : textColors[0];

        if (image)
            image.color = isHovering ? textColors[1] : textColors[0];
    }
}
