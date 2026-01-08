using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class Movement : AttributesSync
{
    [Header("Base setup")]
    [SerializeField] public float walkingSpeed = 0f;
    [SerializeField] public float runningSpeed = 0f;
    [SerializeField] public float jumpSpeed = 0f;
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] public float lookSpeed = 2.0f;
    [SerializeField] public float lookXLimit = 20.0f;
    [SerializeField] public GameObject onlinePanel;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public CubeSpawner spawner;

    [Header("Camera and Head")]
    [SerializeField] private float cameraXOffset = -0.007f;
    [SerializeField] private float cameraYOffset = 1.2f;
    [SerializeField] private float cameraZOffset = 0.209f;
    [SerializeField] private Transform headTransform;

    private Camera playerCamera;
    private CharacterController characterController;
    private Alteruna.Avatar _avatar;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [HideInInspector] public bool canMove = true;

    [SynchronizableField]
    private float syncedPitch = 0f;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        if (_avatar == null)
        {
            Debug.LogError("No Alteruna.Avatar component found on player!");
            return;
        }

        if (!_avatar.IsMe)
            return;

        onlinePanel = GameObject.FindWithTag("OnlineCanvas");
        pauseMenu = GameObject.FindWithTag("PausedMenu");

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (onlinePanel != null) onlinePanel.SetActive(false);

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (playerCamera != null)
        {
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(cameraXOffset, cameraYOffset, cameraZOffset);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (_avatar == null)
            return;

        if (!_avatar.IsMe)
        {
            if (headTransform != null)
                headTransform.localRotation = Quaternion.Euler(syncedPitch - 90, -90, 90);
            return;
        }

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (onlinePanel != null) onlinePanel.SetActive(false);
                if (pauseMenu != null) pauseMenu.SetActive(false);
                if (spawner != null) spawner.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (onlinePanel != null) onlinePanel.SetActive(true);
                if (pauseMenu != null) pauseMenu.SetActive(true);
                if (spawner != null) spawner.enabled = false;
            }
        }

        if (Cursor.lockState == CursorLockMode.None)
            return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            if (headTransform != null)
                headTransform.localRotation = Quaternion.Euler(rotationX - 90, -90, 90);

            syncedPitch = rotationX;
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}