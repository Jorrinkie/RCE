using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;  

public class Movement : AttributesSync
{
    [Header("Base setup")]
    public float walkingSpeed = 0f;
    public float runningSpeed = 0f;
    public float jumpSpeed = 0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 20.0f;
    public GameObject onlinePanel;
    public GameObject pauseMenu;
    public CubeSpawner spawner;

    [Header("Camera and Head")]
    [SerializeField] private float cameraYOffset = 32f;
    [SerializeField] private Transform headTransform; // Assign this in the Inspector

    private Camera playerCamera;
    private CharacterController characterController;
    private Alteruna.Avatar _avatar;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [HideInInspector] public bool canMove = true;

    //This variable will be synced across the network by Alteruna
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

        // Setup UI panels
        onlinePanel = GameObject.FindWithTag("OnlineCanvas");
        pauseMenu = GameObject.FindWithTag("PausedMenu");

        //if (pauseMenu != null) pauseMenu.SetActive(false);
        if (onlinePanel != null) onlinePanel.SetActive(false);

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        // Attach and position the camera
        if (playerCamera != null)
        {
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z + 0.5f);
            playerCamera.transform.SetParent(transform);
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // If this avatar is not mine, just apply synced pitch to the head and skip input
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
        // Toggle cursor and menus
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

        // Movement direction
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);

        // Look rotation
        if (canMove && playerCamera != null)
        {
            // Vertical (pitch)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            if (headTransform != null)
                headTransform.localRotation = Quaternion.Euler(rotationX - 90, -90, 90);

            // Update synced pitch so other players see the head move
            syncedPitch = rotationX;

            // Horizontal (yaw)
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
