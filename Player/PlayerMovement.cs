///-------------------------------------------------------------------------
///   Copyright Wired Visions 2019
///   Class:            PlayerMovement
///   Description:      
///   Author:           Mark Botaish, Jr.
///   Contributor(s):   Parker Staszkiewicz
///-------------------------------------------------------------------------

using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : EventListener
{
    [SerializeField] private float _runSpeed = 7.5f;
    [SerializeField] private float _sprintSpeed = 4.5f;
    [SerializeField] [Range(0.1f, 1.0f)] private float strafeScale = 0.65f;
    [SerializeField] private GameObject _cameraRefence = null;
    [SerializeField] private LayerMask JumpableLayer = 0;
    [SerializeField] private float inkDampening = 0.5f;

    private float _movementDamping = 1.0f;

    public float xLookSensitivity { get; set; } = 5f;
    public float yLookSensitivity { get; set; } = 5f;

    private float xRotation;
    private float yRotation;

    private Quaternion currentRotation; 

    private float currentRunSpeed, currentSprintSpeed; 

    private bool shiftSpeed = false;

    private Vector3 _moveVector = Vector3.zero;

    [SerializeField] private Transform groundCheck = null;
    private float radiusToGround = 0.46f;

    [SerializeField] private float gravity = -9.81f;
    public float Gravity { get { return gravity; } }

    [SerializeField] private float jumpHeight = 2f;
    private float jumpVelocity;
    private bool hasJumped = false;

    private Vector3 yOffset = new Vector3(0f, 0.2f, 0f);

    public Vector3 Movement { get; private set; }
    private Vector3 horizontalMovement;
    private Vector3 verticalMovement;

    private CharacterController controller;
    private Camera _camera;

    private Coroutine inkCoroutine = null;
    private Coroutine freezeCoroutine = null;

    public Transform ball;
    private bool rotateWithBall = false;
    private bool isFrozen = false;

    private void OnEnable()
    {
        NetworkItemManager.EventInk += StartInk;
        SwingFuelItem.EventInk += StartInk;
        EventController.AddListener(typeof(NearbyGolfBallMessage), this);
        EventController.AddListener(typeof(NetworkFreeze), this);
        EventController.AddListener(typeof(AMPBallMessage), this);
        EventController.AddListener(typeof(EndAMPBallMessage), this);
    }

    private void OnDisable()
    {
        NetworkItemManager.EventInk -= StartInk;
        SwingFuelItem.EventInk -= StartInk;
        EventController.RemoveListener(typeof(NearbyGolfBallMessage), this);
        EventController.RemoveListener(typeof(NetworkFreeze), this);
        EventController.RemoveListener(typeof(AMPBallMessage), this);
        EventController.RemoveListener(typeof(EndAMPBallMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is NearbyGolfBallMessage golfBallMessage)
        {
            RotateWithBall(golfBallMessage.golfBall?.gameObject);
        }
        else if (e is NetworkFreeze freeze)
        {
            if (freezeCoroutine != null)
                StopCoroutine(freezeCoroutine);

            freezeCoroutine = StartCoroutine(Freeze(freeze.duration));
        }
        else if (e is AMPBallMessage amp)
        {
            if(amp.playerID == GetComponent<EntityHealth>().MyID)
                UpdateMoveSpeed(amp.speedMultiplier);
        }
        else if (e is EndAMPBallMessage endAmp)
        {
            if (endAmp.playerID == GetComponent<EntityHealth>().MyID)
                ChangeMoveSpeedBackToNormal();
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _camera = Camera.main;

        currentRunSpeed = _runSpeed;
        currentSprintSpeed = _sprintSpeed;

        jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
      
    }

    public void UpdateMoveSpeed(float value)
    {
        currentRunSpeed = _runSpeed * value;
        currentSprintSpeed = _sprintSpeed * value;
    }

    public void ChangeMoveSpeedBackToNormal()
    {
        currentRunSpeed = _runSpeed;
        currentSprintSpeed = _sprintSpeed;
    }

    #region DEFAULT_MOVEMENT

    public void MovePlayer(float xInput, float zInput, bool jump)
    {
        if (isFrozen) { return; }
        //RaycastHit hit;
        bool isTouchingFloor = Physics.CheckSphere(groundCheck.position, radiusToGround, JumpableLayer);

        float currentSpeed = shiftSpeed ? currentSprintSpeed : currentRunSpeed;
        currentSpeed *= _movementDamping;

        Vector3 cameraRight = _camera.transform.right;
        Vector3 cameraForward = _camera.transform.forward;

        cameraRight.y = 0;
        cameraForward.y = 0;

        cameraRight.Normalize();
        cameraForward.Normalize();

        _moveVector += cameraRight * xInput * strafeScale;
        _moveVector += cameraForward * zInput;

        Vector2 normal = new Vector2(_moveVector.x, _moveVector.z) * currentSpeed;
        normal = Vector2.ClampMagnitude(normal, currentSpeed);
        // --- Horizontal Movement --- //
        horizontalMovement = new Vector3(normal.x, 0, normal.y);

        // --- Vertical Movement --- //
        if (isTouchingFloor && verticalMovement.y < 0)
        {
            verticalMovement.y = -4f;
            hasJumped = false;
        }

        if (jump && isTouchingFloor && !hasJumped)
        {
            verticalMovement.y = jumpVelocity;
            EventController.FireEvent(new JumpMessage());
            EventController.FireEvent(new TrackSuperlativeMessage(SuperlativeController.Superlative.TheBouncer, SuperlativeController.ConditionFlag.additive, 1));
            hasJumped = true;
        }

        verticalMovement.y += gravity * Time.deltaTime;

        controller.Move(horizontalMovement * Time.deltaTime);
        controller.Move(verticalMovement * Time.deltaTime);

        Movement = controller.velocity;

        _moveVector.x = 0;
        _moveVector.z = 0;
        // Reset shift speed for this frame
        shiftSpeed = false;
    }


    public void ShiftSpeed()
    {
        shiftSpeed = true;
    }

    public void RotatePlayer()
    {
        xRotation += Input.GetAxis("Mouse X") * xLookSensitivity;
        yRotation += Input.GetAxis("Mouse Y") * -yLookSensitivity;
        yRotation = Mathf.Clamp(yRotation, -45, 75);

        if (rotateWithBall)
        {
            transform.RotateAround(ball.position, Vector3.up, Input.GetAxis("Mouse X") * xLookSensitivity);
            currentRotation = Quaternion.Euler(yRotation, transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            Quaternion nextRotation = Quaternion.Euler(yRotation, xRotation, 0);
            currentRotation = Quaternion.Slerp(currentRotation, nextRotation, 0.8f );
        }

        transform.rotation = Quaternion.Euler(0, xRotation, 0);
     
        _cameraRefence.transform.rotation = currentRotation;
    }

    private void RotateWithBall(GameObject ball)
    {
        this.ball = ball != null ? ball.transform : null;
        rotateWithBall = ball != null;

    }

    public void LookAtCenter()
    {
        transform.LookAt(Vector3.zero);
        _cameraRefence.transform.LookAt(Vector3.zero);
        currentRotation = transform.rotation;
        xRotation = transform.rotation.eulerAngles.y;
        yRotation = transform.rotation.eulerAngles.x;
        yRotation = Mathf.Clamp(yRotation, -75, 75);
    }

    #endregion

    public float GetDamping() { return _movementDamping; }

    private void StartInk(float duration)
    {
        _movementDamping = inkDampening;

        if (inkCoroutine != null)
        {
            StopCoroutine(inkCoroutine);
        }

        inkCoroutine = StartCoroutine(EndInk(duration));
    }

    private System.Collections.IEnumerator EndInk(float duration)
    {
        yield return new WaitForSeconds(duration);
        _movementDamping = 1;
    }

    private System.Collections.IEnumerator Freeze(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }
}
