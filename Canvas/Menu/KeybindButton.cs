using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class KeybindButton : HoverOverAndClick
{
    public GameControls Control { get; private set; }
    [SerializeField] private TMP_Text gameControlText = null;
    [SerializeField] private TMP_Text keyBindText = null;
    [SerializeField] private Color[] textColors = null;

    public void SetUp(float yPos, GameControls control, string keybind)
    {
        Control = control;
        gameControlText.text = control.ToString().AddSpacesBetweenCamelCase();
        keyBindText.text = keybind;

        GetComponent<RectTransform>().localPosition = new Vector2(0, yPos);
    }

    public void SetKeybindText(string text)
    {
        keyBindText.text = text;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        gameControlText.color = textColors[2];
        keyBindText.color = textColors[2];
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        gameControlText.color = textColors[1];
        keyBindText.color = textColors[1];

        isHovering = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        gameControlText.color = textColors[0];
        keyBindText.color = textColors[0];

        isHovering = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        gameControlText.color = isHovering ? textColors[1] : textColors[0];
        keyBindText.color = isHovering ? textColors[1] : textColors[0];

        if (isHovering)
        {
            EventController.FireEvent(new ControlRebindMessage(Control));
        }
    }
}
