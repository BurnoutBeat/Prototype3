
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    [Header("LOOKING")]
    public GameObject eyes; //camera
    public Slider jumpChargeMeter;
    public float maxLookAngle = 80f;
    public float rotationSpeed = 5f;

    [Space(10)]
    [Header("MOVING")]
    public float moveSpeed = 5f;
    public float airMoveSpeed = 2.5f;
    public float jumpForce = 10f;

    [Space(10)]
    [Header("CROUCHING")]
    public float crouchChargeTime = 2f;
    public float maxCrouchJumpPower = 10f;
    public bool infiniteSlide = true;

    [Space(10)]
    [Header("Dashing")]
    [SerializeField] GameObject dashIcon;
    [SerializeField] GameObject noDashIcon;
    public float dashCooldown = 1f;

    [Space(10)]
    [Header("PAUSE MENU")]
    [SerializeField] private Slider sensSlider;
    [SerializeField] private GameObject pauseMenu;

    private PlayerControls inputActions;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private PlayerAbilities playerAbilities;
    private Vector2 moveInput;
    private Vector2 lookDelta;
    private Vector3 lastVelocity;
    private bool crouching = false;
    private bool crouchingMovment = false;
    private bool capsLockHeld = false;
    private bool canDash = true;
    private float crouchStartTime;
    private float verticalRotation = 0f;
    private float chargeStrength;

    private void Awake()
    {
        Time.timeScale = 1;
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        playerAbilities = GetComponent<PlayerAbilities>();
        LoadSensitivity();
    }
    private void FixedUpdate()
    {
        MovePlayer();
        UpdateJumpChargeSlider();
    }
    private void UpdateJumpChargeSlider()
    {
        if (crouching)
        {
            chargeStrength = (Time.time - crouchStartTime) / crouchChargeTime;
            if (chargeStrength > 1)
            {
                chargeStrength = 1;
            }
            jumpChargeMeter.value = chargeStrength * 100;
        }
        else
        {
            jumpChargeMeter.value = 0;
        }
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        print("moved");
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        if (!crouching)
        {
            rb.velocity = new Vector3(rb.velocity.x / 4f, rb.velocity.y, rb.velocity.z / 4f);
        }
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>();
        RotatePlayer(lookDelta);
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (grounded() && (crouching && !crouchingMovment))
        {
            rb.AddForce(Vector3.up * (jumpForce + (maxCrouchJumpPower * chargeStrength)), ForceMode.Impulse);
        }
        else if (grounded() && !crouchingMovment)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        if (crouching && !crouchingMovment)
        {
            StandUp(lastVelocity);
        }
    }
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        capsLockHeld = true;
        crouchStartTime = Time.time;
        if (grounded())
        {
            lastVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
            capsuleCollider.height = 1;
            capsuleCollider.center = new Vector3(0, -0.5f, 0);
            crouching = true;
            Vector3 newPos = transform.position;
            newPos.y -= 0.5f;
            eyes.transform.position = newPos;
        }
    }
    private void CrouchCancled(InputAction.CallbackContext context)
    {
        capsLockHeld = false;
        if (crouching && CanUncrouch())
        {
            StandUp(Vector3.zero);
        }
        else if (crouching)
        {
            crouchingMovment = true;
        }
    }
    private void StandUp(Vector3 vel)
    {
        capsuleCollider.height = 2;
        capsuleCollider.center = Vector3.zero;
        crouching = false;
        crouchingMovment = false;
        Vector3 newPos = transform.position;
        newPos.y += 0.5f;
        eyes.transform.position = newPos;
        rb.velocity = vel;
    }
    private void OnDash(InputAction.CallbackContext ctx)
    {
        if(canDash)
        {
            playerAbilities.Dash();
            dashIcon.SetActive(false);
            noDashIcon.SetActive(true);
            canDash = false;
            StartCoroutine(DashCooldown());
        }
    }
    private bool CanUncrouch()
    {
        float rayDistance = 0.6f;
        Vector3 BoxSize = new Vector3(0.4f, 0.5f, 0.4f);
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        RaycastHit[] hits = Physics.BoxCastAll(centerPos, BoxSize, Vector3.up, Quaternion.identity, rayDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                return false;
            }
        }
        return true;
    }
    private bool grounded()
    {
        float rayDistance = 0.6f;
        Vector3 BoxSize = new Vector3(0.4f, 0.5f, 0.4f);
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        RaycastHit[] hits = Physics.BoxCastAll(centerPos, BoxSize, Vector3.down, Quaternion.identity, rayDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }
    private void MovePlayer()
    {
        Vector3 moveDirection = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        moveDirection.Normalize();
        if (CanUncrouch() && !capsLockHeld && crouching)
        {
            StandUp(Vector3.zero);
        }
        if (crouching)
        {
            if (infiniteSlide)
            {
                rb.velocity = lastVelocity;
            }
            if (crouchingMovment)
            {
                rb.AddForce(moveDirection * moveSpeed * 40f * Time.fixedDeltaTime);
            }
        }
        else
        {

            if (grounded())
            {
                rb.AddForce(moveDirection * moveSpeed * 100f * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(moveDirection * airMoveSpeed * 100f * Time.fixedDeltaTime);
            }
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (flatVelocity.magnitude > moveSpeed)
            {
                rb.velocity = new Vector3(flatVelocity.normalized.x * moveSpeed, rb.velocity.y, flatVelocity.normalized.z * moveSpeed);
            }
        }
    }
    private void RotatePlayer(Vector2 lookInput)
    {
        if (lookInput.sqrMagnitude > 0.01f)
        {
            float rotationAmount = lookInput.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            verticalRotation -= lookInput.y * rotationSpeed * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            eyes.GetComponent<Transform>().localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
    private void OnEnable()
    {
        inputActions.PlayerActions.Enable();
        inputActions.PlayerActions.Move.performed += OnMovePerformed;
        inputActions.PlayerActions.Move.canceled += OnMoveCanceled;
        inputActions.PlayerActions.Jump.performed += OnJump;
        inputActions.PlayerActions.Crouch.performed += CrouchPerformed;
        inputActions.PlayerActions.Crouch.canceled += CrouchCancled;
        inputActions.PlayerActions.Look.performed += OnLook;
        inputActions.PlayerActions.Dash.performed += OnDash;
        inputActions.PlayerActions.Pause.started += Pause_started;
    }
    private void OnDisable()
    {
        inputActions.PlayerActions.Disable();
        inputActions.PlayerActions.Move.performed -= OnMovePerformed;
        inputActions.PlayerActions.Jump.performed -= OnJump;
        inputActions.PlayerActions.Move.canceled -= OnMoveCanceled;
        inputActions.PlayerActions.Crouch.performed -= CrouchPerformed;
        inputActions.PlayerActions.Crouch.canceled -= CrouchCancled;
        inputActions.PlayerActions.Look.performed -= OnLook;
        inputActions.PlayerActions.Dash.performed -= OnDash;
        inputActions.PlayerActions.Pause.started -= Pause_started;
    }
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);

        dashIcon.SetActive(true);
        noDashIcon.SetActive(false);
        canDash = true;
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    /// <param name="obj"></param>
    private void Pause_started(InputAction.CallbackContext obj)
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void ResumeButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Returns to the main menu screen
    /// </summary>
    public void ReturnToMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Sets the sensitivity of the player
    /// </summary>
    /// <param name="slider"></param>
    public void SetSensitivity()
    {
        rotationSpeed = sensSlider.value;
        PlayerPrefs.SetFloat("sens", rotationSpeed);
    }

    /// <summary>
    /// Loads the playerPref of the sensitivity
    /// </summary>
    private void LoadSensitivity()
    {
        rotationSpeed = PlayerPrefs.GetFloat("sens");
        sensSlider.value = rotationSpeed;
    }
}