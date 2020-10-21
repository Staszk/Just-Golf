using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSettingsController : MonoBehaviour
{
    // General
    [SerializeField] private GameObject container = null;
    [SerializeField] private GameObject[] menuScreens = null;
    [SerializeField] private HoverOver[] menuButtons = null;
    private int activeScreenIndex;

    // Controls Menu
    [SerializeField] private ControlsMenu controlsMenu = null;
    
    #region General

    void Start()
    {
        // Handle Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set Up
        container.SetActive(false);
        menuScreens[0].SetActive(true);
        for (int i = 1; i < menuScreens.Length; i++)
        {
            menuScreens[i].SetActive(false);
        }

        controlsMenu.SetUp();

    }

    private void Update()
    {
        if (KeybindingController.GetInputDown(GameControls.Escape))
        {
            EscapePressed();
        }
    }

    public void EscapePressed()
    {
        if (!container.activeInHierarchy) // Become Active
        {
            Activate(true);
            EventController.FireEvent(new SettingsMenuToggleMessage(true));
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (activeScreenIndex == 0) // Become Inactive
        {
            Activate(false);
            EventController.FireEvent(new SettingsMenuToggleMessage(false));
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (controlsMenu.Rebinding)
        {
            controlsMenu.StopRebindProcess();
        }
        else // Exit Menu Layer
        {
            ChangeActiveMenu(0);
        }
    }

    public void Activate(bool active)
    {
        container.SetActive(active);

        if (active)
        {

        }
        else
        {
            foreach (HoverOver hover in menuButtons)
            {
                hover.OnPointerExit(null);
            }
        }
    }

    public void ChangeActiveMenu(int index)
    {
        menuScreens[activeScreenIndex].SetActive(false);
        menuScreens[index].SetActive(true);
        activeScreenIndex = index;
    }

    public void Quit()
    {
        Debug.LogWarning("Quit the Game");
        Application.Quit();
    }

    #endregion
}
