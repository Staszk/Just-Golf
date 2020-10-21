using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : EventListener
{
    [SerializeField] private GameObject endScreenUI = null;
    [SerializeField] private Image yourSuperlative = null;
    [SerializeField] private Image teammateSuperlative = null;
    [SerializeField] private Image enemy1Superlative = null;
    [SerializeField] private Image enemy2Superlative = null;
    [SerializeField] private Sprite[] superlativeIcons = null;

    //If the end screen is currently showing
    private bool onScreen = false;

    private void OnEnable()
    {
        EventController.AddListener(typeof(ShowEndScreenMessage), this);
        EventController.AddListener(typeof(AssignSuperlativeMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ShowEndScreenMessage), this);
        EventController.RemoveListener(typeof(AssignSuperlativeMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if(e is ShowEndScreenMessage)
        {
            BeginEndScreen();
        }
        else if (e is AssignSuperlativeMessage a)
        {
            AssignSuperlativeIcon(yourSuperlative, a.yourSuperlative);
            AssignSuperlativeIcon(teammateSuperlative, a.teammateSuperlative);
            AssignSuperlativeIcon(enemy1Superlative, a.enemy1Superlative);
            AssignSuperlativeIcon(enemy2Superlative, a.enemy2Superlative);
        }
    }

    private void Update()
    {
        if (onScreen)
        {
            if (KeybindingController.GetInput(GameControls.Jump))
            {
                endScreenUI.SetActive(false);
                //Go to title or something
            }
        }
    }

    public void BeginEndScreen()
    {
        endScreenUI.SetActive(true);
    }

    private void AssignSuperlativeIcon(Image image, SuperlativeController.Superlative superlative)
    {
        image.sprite = superlativeIcons[(int)superlative];
    }
}
