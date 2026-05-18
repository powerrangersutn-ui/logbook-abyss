using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Fall Parameters")]
    [SerializeField] private float gravity = 4.0f;
    [SerializeField] private float jumpForce = 5.0f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footsteepSource;
    [SerializeField] private AudioClip[] footsteepSounds;
    [SerializeField] private float walkStepInterval = 1f;
    [SerializeField] private float sprintStepInterval = 0.5f;
    [SerializeField] private float velocityTreshold = 2.0f;

    [Header("Interacción")]
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset PlayerContorls;

    [Header("Check panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject deathPanel;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Camera mainCamera;
    private CharacterController characterController;
    private PlayerInventory inventory;

    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private float nextStepTime;
    private int lastPlayedIndex = -1;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        inventory = GetComponent<PlayerInventory>();

        var playerMap = PlayerContorls.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");
        sprintAction = playerMap.FindAction("Sprint");
        interactAction = playerMap.FindAction("Interact");  

        if (moveAction != null)
        {
            moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
            moveAction.canceled += context => moveInput = Vector2.zero;
        }

        if (lookAction != null)
        {
            lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
            lookAction.canceled += context => lookInput = Vector2.zero;
        }
    }

    private void OnEnable()
{
    if (moveAction != null) moveAction.Enable();
    if (lookAction != null) lookAction.Enable();
    if (jumpAction != null) jumpAction.Enable();
    if (sprintAction != null) sprintAction.Enable();
    if (interactAction != null) interactAction.Enable();
}

private void OnDisable()
{
    if (moveAction != null) moveAction.Disable();
    if (lookAction != null) lookAction.Disable();
    if (jumpAction != null) jumpAction.Disable();
    if (sprintAction != null) sprintAction.Disable();
    if (interactAction != null) interactAction.Disable();
}

    private void Update()
    {
        if (!pausePanel.activeInHierarchy && !victoryPanel.activeInHierarchy && !deathPanel.activeInHierarchy && !GameManager.Instance.gameEnded)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            HandleMovement();
            HandleRotation();
            HandleJumpAndGravity();
            HandleInteraction();
            HandleFootstep();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMovement()
    {
        float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

        float verticalSpeed = moveInput.y * walkSpeed * speedMultiplier;
        float horizontalSpeed = moveInput.x * walkSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;
    }

    void HandleJumpAndGravity()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (jumpAction.triggered)
            {
                currentMovement.y = jumpForce;

                OxygenSystem oxygen = GetComponent<OxygenSystem>();
                oxygen?.OnJump();
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            // Caja de suministros (Oxígeno / Arpones)
            if (hit.collider.TryGetComponent<PickupBox>(out PickupBox box))
            {
                if (interactAction.triggered)
                {
                    box.OnPickup(inventory);
                    return;
                }
            }

            // Vitácora / Logbook
            if (hit.collider.TryGetComponent<Logbook>(out Logbook logbook))
            {
                if (interactAction.triggered)
                {
                    logbook.Interact();
                    return;
                }
            }
        }
    }

    void HandleFootstep()
    {
        float currentStepInterval = (sprintAction.ReadValue<float>() > 0 ? sprintStepInterval : walkStepInterval);

        if (characterController.isGrounded &&
            (moveInput.y != 0 || moveInput.x != 0) &&
            Time.time > nextStepTime &&
            characterController.velocity.magnitude > velocityTreshold)
        {
            PlayFootstepSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    void PlayFootstepSounds()
    {
        if (footsteepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footsteepSounds.Length);
        if (footsteepSounds.Length > 1 && randomIndex == lastPlayedIndex)
            randomIndex = (randomIndex + 1) % footsteepSounds.Length;

        lastPlayedIndex = randomIndex;
        footsteepSource.clip = footsteepSounds[randomIndex];
        footsteepSource.Play();
    }
}