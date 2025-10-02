using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 1.2f;

    private bool isGrounded;
    private bool isMoving;
    private bool isCrouching;
    private Vector3 velocity;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float sensitivity = 2.5f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Look Settings")]
    [SerializeField] private bool restrictedLook = false;
    [SerializeField] private float minLookX = -90f;
    [SerializeField] private float maxLookX = 90f;
    [SerializeField] private float minLookY = -90f;
    [SerializeField] private float maxLookY = 90f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSound;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;

    private float footstepTimer = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;

        Move();
        Look();
        Crouch();
        HandleFootsteps();
    }

    private void Move()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        isMoving = (x != 0 || z != 0);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        float minClampX = restrictedLook ? minLookX : -90f;
        float maxClampX = restrictedLook ? maxLookX : 90f;
        xRotation = Mathf.Clamp(xRotation, minClampX, maxClampX);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (restrictedLook)
        {
            yRotation += mouseX;
            yRotation = Mathf.Clamp(yRotation, minLookY, maxLookY);
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            transform.Rotate(Vector3.up * mouseX);
            yRotation = transform.localEulerAngles.y;
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cam.transform.localPosition = new Vector3(0, 0.25f, 0);
            characterController.height = 1;
            moveSpeed = 2f;
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cam.transform.localPosition = new Vector3(0, 0.53f, 0);
            characterController.height = 3;
            moveSpeed = 6f;
            isCrouching = false;
        }
    }

    private void HandleFootsteps()
    {
        if (footstepClips.Length == 0 || footstepSound == null)
            return;

        if (isGrounded && isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayRandomFootstep();
                footstepTimer = isCrouching ? stepInterval * 1.5f : stepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayRandomFootstep()
    {
        int index = Random.Range(0, footstepClips.Length);
        footstepSound.clip = footstepClips[index];
        footstepSound.pitch = Random.Range(0.8f, 1.1f);
        footstepSound.Play();
    }
}