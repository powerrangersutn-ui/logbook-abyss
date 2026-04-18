using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed =5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header ("Fall Parameters")]
    [SerializeField] private float gravity = 4.0f;
    //por ahora no hay salto, lo dejo por las dudas
    //[SerializeField] private float jumpForce = 5.0f;

    [Header("Look Sesnsitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;

    //Poner sonidos de pasos nunca fué tam difi-, digo, fácil :D
    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footsteepSource;
    [SerializeField] private AudioClip[] footsteepSounds;
    [SerializeField] private float walkStepInterval = 1f;
    [SerializeField] private float sprintStepInterval = 0.5f;
    [SerializeField] private float velocityTreshold = 2.0f;

    [Header ("Input Actions")]
    [SerializeField] private InputActionAsset PlayerContorls;

    [Header ("Check panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject deathPanel;

    private int lastPlayedIndex = -1;
    private bool isMoving;
    private float nextStepTime;
    private Camera mainCamera;
    private float verticalRotation;
    private Vector3 currentMovement=Vector3.zero;
    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction lookAction;
    //private InputAction jumpAction;
    private InputAction sprintAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera=Camera.main;

        moveAction = PlayerContorls.FindActionMap("Player").FindAction("Move");
        lookAction = PlayerContorls.FindActionMap("Player").FindAction("Look");
        //jumpAction = PlayerContorls.FindActionMap("Player").FindAction("Jump");
        sprintAction = PlayerContorls.FindActionMap("Player").FindAction("Sprint");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        //jumpAction.Enable();
        sprintAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        //jumpAction.Disable();
        sprintAction.Disable();
    }

    private void Update()
    {
        if (!pausePanel.activeInHierarchy && !victoryPanel.activeInHierarchy && !deathPanel.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HandleMovement();
            HandleRotation();
            HandleFootstep();
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    void HandleMovement()
    {
        float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f; //Uso de sugar code, si apreto la tecla, speedMultiplier vale sprintMultiplier, si no vale 1

        float verticalSpeed = moveInput.y * walkSpeed * speedMultiplier;
        float horizontalSpeed = moveInput.x * walkSpeed * speedMultiplier;

        //Vector3 horizontalMovement = new Vector3 (horizontalInput, 0, verticalInput);
        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement.Normalize();
        horizontalMovement = transform.rotation * horizontalMovement * walkSpeed * speedMultiplier;

        HandleGravity();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = moveInput.y != 0 || moveInput.x!= 0;
    }

    void HandleGravity()
    {
        if (characterController.isGrounded)
            currentMovement.y= -0.5f;
        else
            currentMovement.y -= gravity * Time.deltaTime;
    }


    //por si agregamos un salto
    /*void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (jumpAction.triggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y-= gravity * Time.deltaTime;
        }
    }*/

    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0,mouseXRotation,0);

        verticalRotation -= lookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation,0,0);
    }

    void HandleFootstep()
    {
        float currentStepInterval = (sprintAction.ReadValue<float>() > 0 ? sprintStepInterval : walkStepInterval);
        if(characterController.isGrounded && isMoving && Time.time >nextStepTime && characterController.velocity.magnitude > velocityTreshold)
        {
            PlayFootstepSounds();
            nextStepTime = Time.time+currentStepInterval;
        }
    }

    void PlayFootstepSounds()
    {
        int randomIndex; //por si tenemos varios sonidos de pasos
        if (footsteepSounds.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = (int)Random.Range(0, footsteepSounds.Length - 1);
            if (randomIndex >= lastPlayedIndex)
            {
                randomIndex++;
            }
        }

        lastPlayedIndex = randomIndex;
        footsteepSource.clip = footsteepSounds[randomIndex];
        footsteepSource.Play();
    }
}
