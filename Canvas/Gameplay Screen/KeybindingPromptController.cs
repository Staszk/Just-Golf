using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct Prompt
{
    public GameControls control;
    public TMPro.TMP_Text textPrompt;
}

public class KeybindingPromptController : EventListener
{
    [SerializeField] private Prompt[] prompts = null;

    [SerializeField] private Color fadedColor = Color.white;

    public void Awake()
    {
        //ShowPrompt();

        //FadePrompt(System.Array.Find(prompts, prompt => prompt.control == GameControls.UseSpecialItem), true);
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(UpdateControlsMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(UpdateControlsMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is UpdateControlsMessage)
        {
            ShowPrompt();
        }
    }

    private void ShowPrompt()
    {
        foreach (Prompt item in prompts)
        {
            item.textPrompt.text = KeybindingController.GetKeybindName(item.control);
        }
    }

    public void FadeClubPowerPrompt(bool fade)
    {
        Prompt p = System.Array.Find(prompts, prompt => prompt.control == GameControls.SecondaryUse);

        FadePrompt(p, fade);
    }

    private void FadePrompt(Prompt p, bool fade)
    {
        p.textPrompt.color = fade ? fadedColor : Color.white;
    }
}
