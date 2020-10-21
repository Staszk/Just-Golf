using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSettings : MonoBehaviour
{
    #region Singleton

    private static ClientSettings instance = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (transform.parent)
            DontDestroyOnLoad(transform.parent);
        else
            DontDestroyOnLoad(transform);

        SetUp();
    }

    #endregion

    public static float StandardSensitivity { get { return instance.standardMouseSensitivity; } }
    public static bool RotateMinimap { get { return instance.rotateMinimap; } }

    private float standardMouseSensitivity = 0.3f;
    private bool rotateMinimap = true;

    private void SetUp()
    {
        KeybindingController.Init();

        // In Future, get Sensitivities from a save file
    }

    public static void UpdateSensitivity(int index, float value)
    {
        switch (index)
        {
            case 0:
                instance.standardMouseSensitivity = value / 50.0f;
                break;
            default:
                break;
        }
    }

    public void ToggleMinimapRotate(bool active)
    {
        instance.rotateMinimap = active;
    }
}
