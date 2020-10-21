using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : EventListener
{
    public float xStandardSpeed = 5;
    public float yStandardSpeed = 5;

    private CinemachineFreeLook virtualCam;
    [SerializeField] private float golfFOV = 65f;
    [SerializeField] private float shootFOV = 65f;

    private float xZoomSpeed, yZoomSpeed;

    private Coroutine cameraZoomCoroutine;

    private bool isActive = true;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

        virtualCam = GetComponent<CinemachineFreeLook>();
        virtualCam.m_Lens.FieldOfView = golfFOV;

        xZoomSpeed = xStandardSpeed * 0.4f;
        yZoomSpeed = yStandardSpeed * 0.4f;

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerMovement.xLookSensitivity = xStandardSpeed;
        playerMovement.yLookSensitivity = yStandardSpeed;
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(SettingsMenuToggleMessage), this);
        EventController.AddListener(typeof(ModeChangedMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(SettingsMenuToggleMessage), this);
        EventController.RemoveListener(typeof(ModeChangedMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ModeChangedMessage modeChange)
        {
            ChangeFOV(modeChange);
        }
        else if (e is SettingsMenuToggleMessage)
        {
            AcceptInput();
        }
    }

    private void AcceptInput()
    {
        isActive = !isActive;

        if (!isActive)
        {
            playerMovement.xLookSensitivity = 0;
            playerMovement.yLookSensitivity = 0;
        }
        else
        {
            playerMovement.xLookSensitivity = xStandardSpeed * ClientSettings.StandardSensitivity;
            playerMovement.yLookSensitivity = yStandardSpeed * ClientSettings.StandardSensitivity;
        }
    }

    private void PrepareToggleZoom(FOVChangeMessage eventMessage)
    {
        float fromFOV;
        float targetFOV;

        if (eventMessage.zoomLayer == 0) // zoom all the way out (golf)
        {
            targetFOV = golfFOV;
            fromFOV = shootFOV;

            if (isActive) //check if we are active, because we may have zoomed out for the menus
            {
                playerMovement.xLookSensitivity = xStandardSpeed * ClientSettings.StandardSensitivity;
                playerMovement.yLookSensitivity = yStandardSpeed * ClientSettings.StandardSensitivity;
            }

        }
        else // shoot mode
        {
            targetFOV = shootFOV;
            fromFOV = golfFOV;

            if (isActive) //check if we are active, because we may have zoomed out for the menus
            {
                playerMovement.xLookSensitivity = xStandardSpeed * ClientSettings.StandardSensitivity;
                playerMovement.yLookSensitivity = yStandardSpeed * ClientSettings.StandardSensitivity;
            }
        }

        if (cameraZoomCoroutine != null)
        {
            StopCoroutine(cameraZoomCoroutine);
        }

        cameraZoomCoroutine = StartCoroutine(ToggleZoom(fromFOV, targetFOV, eventMessage.zoomSpeed));
    }

    private void ChangeFOV(ModeChangedMessage eventMessage)
    {
        if (eventMessage.modeChangedToGolf)
        {
            PrepareToggleZoom(new FOVChangeMessage(0.275f, false, 0));
        }
        else
        {
            PrepareToggleZoom(new FOVChangeMessage(0.35f, true, 1));
        }
    }

    private IEnumerator ToggleZoom(float fromFOV, float targetFOV, float zoomSpeed)
    {
        float fov = virtualCam.m_Lens.FieldOfView;
        float defaultMagnitude = Mathf.Abs(fromFOV - targetFOV);
        float magnitude = Mathf.Abs(fov - targetFOV);

        float time = 0;
        float maxTime = zoomSpeed * (magnitude / defaultMagnitude);

        while (time != maxTime)
        {
            time = Mathf.Min(time + Time.deltaTime, maxTime);

            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(fov, targetFOV, time / maxTime);
            yield return null;
        }

        // Force set in case lerp was not accurate
        virtualCam.m_Lens.FieldOfView = targetFOV;
        //EventController.FireEvent(new CameraTransitionMessage());
    }
}
