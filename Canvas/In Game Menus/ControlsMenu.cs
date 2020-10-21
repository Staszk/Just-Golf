using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : EventListener
{
    [SerializeField] private KeybindButton keybindButtonPrefab = null;
    [SerializeField] private RectTransform contentWindow = null;
    [SerializeField] private GameObject uiBlocker = null;
    [SerializeField] private GameObject keybindExplanation = null;

    private List<KeybindButton> keybindButtons;

    private string[] controlSchemes = new string[] { "Righty", "Lefty"};
    [SerializeField] private TMPro.TMP_Text controlscheme = null;
    private int controlsIndex = 0;
    private readonly int defaultControlsCount = 2;

    // Rebinding
    public bool Rebinding { get; private set; }
    private GameControls controlToBind;

    private void OnEnable()
    {
        EventController.AddListener(typeof(ControlRebindMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ControlRebindMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ControlRebindMessage rebindMessage)
        {
            StartRebindProcess(rebindMessage);
        }
    }

    public void SetUp()
    {
        SetUpControlsMenu();
    }

    private void SetUpControlsMenu()
    {
        keybindButtons = new List<KeybindButton>();

        int controlsCount = System.Enum.GetValues(typeof(GameControls)).Length;
        float yScale = keybindButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;

        contentWindow.sizeDelta = new Vector2(contentWindow.sizeDelta.x, (controlsCount - 1) * yScale); // count - 1 so ESCAPE can't be rebound

        for (int i = 0; i < controlsCount - 1; i++)
        {
            GameControls c = (GameControls)i;
            KeybindButton keybindButton = Instantiate(keybindButtonPrefab, contentWindow);
            keybindButton.SetUp(-yScale * i, c, KeybindingController.GetKeybindName(c));

            keybindButtons.Add(keybindButton);
        }

        uiBlocker.SetActive(false);
        keybindExplanation.SetActive(false);
    }

    public void CycleDefaultControls(int dir)
    {
        controlsIndex += dir;

        if (controlsIndex < 0)
            controlsIndex = defaultControlsCount - 1;
        else if (controlsIndex >= defaultControlsCount)
            controlsIndex = 0;

        controlscheme.text = controlSchemes[controlsIndex];

        KeybindingController.SetControls(controlsIndex);

        foreach (KeybindButton kb in keybindButtons)
        {
            kb.SetKeybindText(KeybindingController.GetKeybindName(kb.Control));
        }
    }

    public void StartRebindProcess(ControlRebindMessage eventMessage)
    {
        controlToBind = eventMessage.control;
        Rebinding = true;
        uiBlocker.SetActive(true);
        keybindExplanation.SetActive(true);
    }

    public void StopRebindProcess()
    {
        Rebinding = false;

        foreach (KeybindButton keybindButton in keybindButtons)
        {
            keybindButton.SetKeybindText(KeybindingController.GetKeybindName(keybindButton.Control));
        }

        StartCoroutine(RemoveUIBlocker());
    }

    private IEnumerator RemoveUIBlocker()
    {
        yield return new WaitForSeconds(0.05f);
        uiBlocker.SetActive(false);
        keybindExplanation.SetActive(false);
    }

    public void OnGUI()
    {
        if (Rebinding)
        {
            Event e = Event.current;

            if (e.isKey && e.keyCode != KeyCode.Escape)
            {
                KeyCode kc = e.keyCode;

                KeybindingController.SetControlKeybind(controlToBind, kc);

                StopRebindProcess();
            }
            else if (e.shift)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    KeybindingController.SetControlKeybind(controlToBind, KeyCode.LeftShift);
                else
                    KeybindingController.SetControlKeybind(controlToBind, KeyCode.RightShift);

                StopRebindProcess();
            }
            else if (e.isMouse)
            {
                switch (e.button)
                {
                    case 0:
                        KeybindingController.SetControlKeybind(controlToBind, MouseCode.LeftMouseButton);
                        break;
                    case 1:
                        KeybindingController.SetControlKeybind(controlToBind, MouseCode.RightMouseButton);
                        break;
                    case 2:
                        KeybindingController.SetControlKeybind(controlToBind, MouseCode.MiddleMouseButton);
                        break;
                    default:
                        Debug.Log(e.button);
                        break;
                }

                StopRebindProcess();
            }
            else if (e.isScrollWheel)
            {
                if (e.delta.y < 0) // scroll up
                {
                    KeybindingController.SetControlKeybind(controlToBind, MouseCode.MouseWheelUp);
                }
                else // scroll down
                {
                    KeybindingController.SetControlKeybind(controlToBind, MouseCode.MouseWheelDown);
                }

                StopRebindProcess();
            }
        }
    }
}
