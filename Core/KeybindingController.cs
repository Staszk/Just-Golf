using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameControls
{
    MoveForward,
    StrafeLeft,
    MoveBackward,
    StrafeRight,
    Sprint,
    Jump,
    PayRespects,
    PrimaryUse,
    SecondaryUse,
    NextClub,
    PreviousClub,
    SwitchToClubOne,
    SwitchToClubTwo,
    SwitchToClubThree,
    AbilityBallOne,
    AbilityBallTwo,
    AbilityBallThree,
    ShowScoreboard,
    Escape
    // DO NOT add to the bottom of the list
}

public enum MouseCode
{
    None,
    LeftMouseButton,
    RightMouseButton,
    MiddleMouseButton,
    MouseWheelUp,
    MouseWheelDown,
}

public static class KeybindingController
{
    private struct KeyBind
    {
        public KeyCode keyCode;
        public MouseCode mouseCode;

        public KeyBind(KeyCode k, MouseCode m)
        {
            keyCode = k;
            mouseCode = m;
        }
    }

    private static Dictionary<GameControls, KeyBind> keyValuePairs;
    private static Dictionary<GameControls, KeyBind>[] defaults;
    private static bool initialized = false;

    public static void Init()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        defaults = new Dictionary<GameControls, KeyBind>[2];

        defaults[0] = new Dictionary<GameControls, KeyBind>
        {
            // Read from a save file in the future
            { GameControls.MoveForward, new KeyBind(KeyCode.W, MouseCode.None) },
            { GameControls.StrafeLeft, new KeyBind(KeyCode.A, MouseCode.None) },
            { GameControls.MoveBackward, new KeyBind(KeyCode.S, MouseCode.None) },
            { GameControls.StrafeRight, new KeyBind(KeyCode.D, MouseCode.None) },
            { GameControls.Sprint, new KeyBind(KeyCode.LeftShift, MouseCode.None) },
            { GameControls.Jump, new KeyBind(KeyCode.Space, MouseCode.None) },
            { GameControls.PayRespects, new KeyBind(KeyCode.O, MouseCode.None) },
            { GameControls.PrimaryUse, new KeyBind(KeyCode.None, MouseCode.LeftMouseButton) },
            { GameControls.SecondaryUse, new KeyBind(KeyCode.None, MouseCode.RightMouseButton) },
            { GameControls.AbilityBallOne, new KeyBind(KeyCode.E, MouseCode.RightMouseButton) },
            { GameControls.AbilityBallTwo, new KeyBind(KeyCode.Q, MouseCode.RightMouseButton) },
            { GameControls.AbilityBallThree, new KeyBind(KeyCode.F, MouseCode.RightMouseButton) },
            { GameControls.SwitchToClubOne, new KeyBind(KeyCode.Alpha1, MouseCode.None) },
            { GameControls.SwitchToClubTwo, new KeyBind(KeyCode.Alpha2, MouseCode.None) },
            { GameControls.SwitchToClubThree, new KeyBind(KeyCode.Alpha3, MouseCode.None) },
            { GameControls.NextClub, new KeyBind(KeyCode.None, MouseCode.MouseWheelDown) },
            { GameControls.PreviousClub, new KeyBind(KeyCode.None, MouseCode.MouseWheelUp) },
            { GameControls.ShowScoreboard, new KeyBind(KeyCode.Tab, MouseCode.None) },
            { GameControls.Escape, new KeyBind(KeyCode.Escape, MouseCode.None) }
        };

        defaults[1] = new Dictionary<GameControls, KeyBind>
        {
            // Read from a save file in the future
            { GameControls.MoveForward, new KeyBind(KeyCode.I, MouseCode.None) },
            { GameControls.StrafeLeft, new KeyBind(KeyCode.J, MouseCode.None) },
            { GameControls.MoveBackward, new KeyBind(KeyCode.K, MouseCode.None) },
            { GameControls.StrafeRight, new KeyBind(KeyCode.L, MouseCode.None) },
            { GameControls.Sprint, new KeyBind(KeyCode.RightShift, MouseCode.None) },
            { GameControls.Jump, new KeyBind(KeyCode.Space, MouseCode.None) },
            { GameControls.PayRespects, new KeyBind(KeyCode.F, MouseCode.None) },
            { GameControls.PrimaryUse, new KeyBind(KeyCode.None, MouseCode.RightMouseButton) },
            { GameControls.SecondaryUse, new KeyBind(KeyCode.None, MouseCode.LeftMouseButton) },
            { GameControls.AbilityBallOne, new KeyBind(KeyCode.U, MouseCode.LeftMouseButton) },
            { GameControls.AbilityBallTwo, new KeyBind(KeyCode.O, MouseCode.LeftMouseButton) },
            { GameControls.AbilityBallThree, new KeyBind(KeyCode.Semicolon, MouseCode.LeftMouseButton) },
            { GameControls.SwitchToClubOne, new KeyBind(KeyCode.Minus, MouseCode.None) },
            { GameControls.SwitchToClubTwo, new KeyBind(KeyCode.Alpha0, MouseCode.None) },
            { GameControls.SwitchToClubThree, new KeyBind(KeyCode.Alpha9, MouseCode.None) },
            { GameControls.NextClub, new KeyBind(KeyCode.None, MouseCode.MouseWheelDown) },
            { GameControls.PreviousClub, new KeyBind(KeyCode.None, MouseCode.MouseWheelUp) },
            { GameControls.ShowScoreboard, new KeyBind(KeyCode.LeftBracket, MouseCode.None) },
            { GameControls.Escape, new KeyBind(KeyCode.Escape, MouseCode.None) }
        };

        SetControls(0);

        foreach (GameControls controls in System.Enum.GetValues(typeof(GameControls)))
        {
            if (!keyValuePairs.ContainsKey(controls))
            {
                keyValuePairs.Add(controls, new KeyBind(KeyCode.None, MouseCode.None));
            }
        }
    }

    public static void SetControls(int index)
    {
        keyValuePairs = defaults[index];

        EventController.FireEvent(new UpdateControlsMessage());
    }

    public static bool GetInputDown(GameControls key)
    {
        if (keyValuePairs[key].keyCode != KeyCode.None)
        {
            return Input.GetKeyDown(keyValuePairs[key].keyCode);
        }
        else if (keyValuePairs[key].mouseCode != MouseCode.None)
        {
            switch (keyValuePairs[key].mouseCode)
            {
                case MouseCode.LeftMouseButton:
                    return Input.GetMouseButtonDown(0);
                case MouseCode.RightMouseButton:
                    return Input.GetMouseButtonDown(1);
                case MouseCode.MiddleMouseButton:
                    return Input.GetMouseButtonDown(2);
                case MouseCode.MouseWheelUp:
                    return Input.mouseScrollDelta.y > 0;
                case MouseCode.MouseWheelDown:
                    return Input.mouseScrollDelta.y < 0;
                default:
                    return false;
            }
        }

        // The Game Control is not set, return false
        return false;
    }

    public static bool GetInput(GameControls key)
    {
        if (keyValuePairs[key].keyCode != KeyCode.None)
        {
            return Input.GetKey(keyValuePairs[key].keyCode);
        }
        else if (keyValuePairs[key].mouseCode != MouseCode.None)
        {
            switch (keyValuePairs[key].mouseCode)
            {
                case MouseCode.LeftMouseButton:
                    return Input.GetMouseButton(0);
                case MouseCode.RightMouseButton:
                    return Input.GetMouseButton(1);
                case MouseCode.MiddleMouseButton:
                    return Input.GetMouseButton(2);
                case MouseCode.MouseWheelUp:
                    return Input.mouseScrollDelta.y > 0;
                case MouseCode.MouseWheelDown:
                    return Input.mouseScrollDelta.y < 0;
                default:
                    return false;
            }
        }

        // The Game Control is not set, return false
        return false;
    }

    public static bool GetInputUp(GameControls key)
    {
        if (keyValuePairs[key].keyCode != KeyCode.None)
        {
            return Input.GetKeyUp(keyValuePairs[key].keyCode);
        }
        else if (keyValuePairs[key].mouseCode != MouseCode.None)
        {
            switch (keyValuePairs[key].mouseCode)
            {
                case MouseCode.LeftMouseButton:
                    return Input.GetMouseButtonUp(0);
                case MouseCode.RightMouseButton:
                    return Input.GetMouseButtonUp(1);
                case MouseCode.MouseWheelUp:
                    return Input.mouseScrollDelta.y == 0;
                case MouseCode.MouseWheelDown:
                    return Input.mouseScrollDelta.y == 0;
                default:
                    return false;
            }
        }

        // The Game Control is not set, return false
        return false;
    }

    public static bool SetControlKeybind(GameControls control, KeyCode key)
    {
        if (keyValuePairs.ContainsKey(control))
        {
            // Clear Keybind from other controls
            ClearKeybind(key);

            // Set new Keybind
            keyValuePairs[control] = new KeyBind(key, MouseCode.None);

            EventController.FireEvent(new UpdateControlsMessage());

            return true;
        }

        return false;
    }

    public static bool SetControlKeybind(GameControls control, MouseCode mouse)
    {
        if (keyValuePairs.ContainsKey(control))
        {
            // Clear Keybind from other controls
            ClearKeybind(mouse);

            // Set new Keybind
            keyValuePairs[control] = new KeyBind(KeyCode.None, mouse);

            EventController.FireEvent(new UpdateControlsMessage());

            return true;
        }

        return false;
    }

    public static void ClearKeybind(KeyCode key)
    {
        foreach (KeyValuePair<GameControls, KeyBind> kvp in keyValuePairs)
        {
            if (kvp.Value.keyCode == key)
            {
                keyValuePairs[kvp.Key] = new KeyBind(KeyCode.None, MouseCode.None);
                return;
            }
        }
    }

    public static void ClearKeybind(MouseCode mouse)
    {
        foreach (KeyValuePair<GameControls, KeyBind> kvp in keyValuePairs)
        {
            if (kvp.Value.mouseCode == mouse)
            {
                keyValuePairs[kvp.Key] = new KeyBind(KeyCode.None, MouseCode.None);
                return;
            }
        }
    }

    public static string GetKeybindName(GameControls c)
    {
        if (!initialized)
        {
            Init();
        }

        KeyCode k = keyValuePairs[c].keyCode;
        MouseCode m = keyValuePairs[c].mouseCode;

        if (k != KeyCode.None)
        {
            return GetSpecificName(k.ToString().AddSpacesBetweenCamelCase()).RemoveWords();
        }
        else if (m != MouseCode.None)
        {
            return GetSpecificName(m.ToString().AddSpacesBetweenCamelCase()).RemoveWords();
        }

        return "";
    }

    public static string GetSpecificName(string text)
    {
        switch (text)
        {
            case "Left Mouse Button":
                return "LMB";
            case "Right Mouse Button":
                return "RMB";
            case "Mouse Wheel Down":
                return "Scroll Down";
            case "Mouse Wheel Up":
                return "Scroll Up";
            case "Minus":
                return "-";
            default:
                return text;
        }
    }
}
