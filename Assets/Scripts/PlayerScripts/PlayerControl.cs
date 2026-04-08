using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Fall Parameters")]
    [SerializeField] private float gravity = 1.0f;
    //por ahora no hay salto, lo dejo por las dudas
    //[SerializeField] private float jumpForce = 5.0f;

    [Header("Look Sesnsitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;

    //Esto es para poder cambiar los controles
    [Header("Inputs Customisation")]
    [SerializeField] private string horizontalMoveInput = "Horizontal";
    [SerializeField] private string verticalMoveInput = "Vertical";
    [SerializeField] private string mouseXInput = "Mouse X";
    [SerializeField] private string mouseYInput = "Mouse Y";
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    //por ahora no hay salto, lo dejo por las dudas
    //[SerializeField] private KeyCode jumpKey = KeyCode.Space;

    //Poner sonidos de pasos nunca fué tam difi-, digo, fácil :D
    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footsteepSource;
    [SerializeField] private AudioClip[] footsteepSounds;
    [SerializeField] private float walkStepInterval = 1f;
    [SerializeField] private float sprintStepInterval = 0.5f;
    [SerializeField] private float velocityTreshold = 2.0f;

    private int lastPlayedIndex = -1;
    private bool isMoving;
    private float nextStepTime;
    private Camera mainCamera;
    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFootstep();
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis(verticalMoveInput);
        float horizontalInput = Input.GetAxis(horizontalMoveInput);

        float speedMultiplier = Input.GetKey(sprintKey) ? sprintMultiplier : 1f; //Uso de sugar code, si apreto la tecla, speedMultiplier vale sprintMultiplier, si no vale 1

        //float verticalSpeed = verticalInput * walkSpeed * speedMultiplier;
        //float horizontalSpeed = horizontalInput * walkSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3(horizontalInput, 0, verticalInput);
        horizontalMovement.Normalize();
        horizontalMovement = transform.rotation * horizontalMovement * walkSpeed * speedMultiplier;

        HandleGravity();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = verticalInput != 0 || horizontalInput != 0;
    }

    void HandleGravity()
    {
        if (characterController.isGrounded)
            currentMovement.y = -0.5f;
        else
            currentMovement.y -= gravity * Time.deltaTime;
    }


    //por si agregamos un salto
    /*void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (Input.GetKeyDown(jumpKey))
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
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= Input.GetAxis(mouseYInput) * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleFootstep()
    {
        float currentStepInterval = (Input.GetKey(sprintKey) ? sprintStepInterval : walkStepInterval);
        if (characterController.isGrounded && isMoving && Time.time > nextStepTime && characterController.velocity.magnitude > velocityTreshold)
        {
            PlayFootstepSounds();
            nextStepTime = Time.time + currentStepInterval;
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