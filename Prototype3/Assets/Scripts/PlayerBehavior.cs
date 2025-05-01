
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class PlayerBehavior : MonoBehaviour
{
    public bool usingControler = false;
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
    public float fallingGravity = 2f;

    [Space(10)]
    [Header("CROUCHING")]
    public float crouchChargeTime = 2f;
    public float uncrouchChargeTime = 2f;
    public float maxCrouchJumpPower = 10f;
    public bool infiniteSlide = true;
    public bool boostedCharge = false;

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
    private float crouchStartTime;
    private bool canDash = true;
    private float verticalRotation = 0f;
    private float chargeStrength;
    bool leftGround, moving;

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
        leftGround = false;
        moving = false;
    }
    private void FixedUpdate()
    {
        MovePlayer();
        UpdateJumpChargeSlider();
        if (usingControler)
        {
            LoadSensitivity();
            RotatePlayer(lookDelta);
        }
        ApplyGravity();
        if (ShouldCrouch()) {
            StartCrouch();
        } else {
            EndCrouch();
        }

        if(grounded() && leftGround)
        {
            leftGround = false;
            SoundManager.Instance.PlaySFX("Land");
        }
        else if(!grounded())
        {
            leftGround = true;
        }
    }
    private bool ShouldCrouch() {
        if (grounded() && capsLockHeld)
        {
            lastVelocity = rb.velocity;
            return true;
        }
        return false;
    }
    private void StartCrouch()
    {
        crouching = true;
        lastVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        capsuleCollider.height = 1;
        capsuleCollider.center = new Vector3(0, -0.5f, 0);
        Vector3 newPos = transform.position;
        newPos.y -= 0.5f;
        eyes.transform.position = newPos;
    }
    private void EndCrouch()
    {
        if (CanUncrouch())
        {
            crouching = false;
            StandUp();
        } else if (rb.velocity.magnitude < 0.2f)
        {
            crouchingMovment = true;
        }
    }
    private void ApplyGravity()
    {
        if (rb.velocity.y <= 0) {
            Vector3 gravity = Physics.gravity * fallingGravity;
            rb.AddForce(gravity, ForceMode.Acceleration);
        }
    }
    private void UpdateJumpChargeSlider()
    {
        if (crouching)
        {
            chargeStrength += crouchChargeTime * Time.deltaTime;
            if (chargeStrength > 1)
            {
                chargeStrength = 1;
            } else if (chargeStrength > 1 && boostedCharge)
            {
                chargeStrength = 1.5f;
            }
            jumpChargeMeter.value = chargeStrength * 100;
        }
        else 
        {
            chargeStrength -= uncrouchChargeTime * Time.deltaTime;
            if (chargeStrength < 0)
            {
                chargeStrength = 0;
            }
            jumpChargeMeter.value = chargeStrength * 100;
        }
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        print("moved");
        moveInput = context.ReadValue<Vector2>();
        if(!moving && grounded())
        {
            moving = true;
            SoundManager.Instance.PlaySFX("Walk");
        }
        else if(!grounded())
        {
            moving = false;
            SoundManager.Instance.StopSFX("Walk");
        }
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        SoundManager.Instance.StopSFX("Walk");
        moving = false;
        moveInput = Vector2.zero;
        if (!crouching)
        {
            rb.velocity = new Vector3(rb.velocity.x / 4f, rb.velocity.y, rb.velocity.z / 4f);
        }
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        if (context.control.device is Gamepad) {
            usingControler = true;
        }
        if (context.control.device is Mouse)
        {
            usingControler = false;
        }
        if (!usingControler)
        {
            lookDelta = context.ReadValue<Vector2>();
            LoadSensitivity();
            RotatePlayer(lookDelta);
        }
        else {
            lookDelta = context.ReadValue<Vector2>() * 15f;
        }
    }
    private void LookStopped(InputAction.CallbackContext context)
    {
        lookDelta = Vector2.zero;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeSelf) {
            if (grounded())
            {
                print(chargeStrength);
                rb.AddForce(Vector3.up * (jumpForce + (maxCrouchJumpPower * chargeStrength)), ForceMode.Impulse);
                SoundManager.Instance.PlaySFX("Jump");
            }
            if (crouching && CanUncrouch())
            {
                StandUp();
            }
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
            Vector3 newPos = transform.position;
            newPos.y -= 0.5f;
            eyes.transform.position = newPos;
            crouching = true;

        }
        else if(!grounded())
        {

        }
    }

    private void CrouchCancled(InputAction.CallbackContext context)
    {
        capsLockHeld = false;
        if (crouching && CanUncrouch())
        {
            StandUp();
        }
        else if (crouching)
        {
            crouchingMovment = true;
        }
    }
    private void StandUp()
    {
        crouching = false;
        crouchingMovment = false;
        capsuleCollider.height = 2;
        capsuleCollider.center = Vector3.zero;
        crouching = false;
        crouchingMovment = false;
        Vector3 newPos = transform.position;
        newPos.y += 0.5f;
        eyes.transform.position = newPos;
        boostedCharge = false;
    }
    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (!pauseMenu.activeSelf) {
            if (canDash)
            {
                playerAbilities.Dash();
                dashIcon.SetActive(false);
                noDashIcon.SetActive(true);
                canDash = false;
                StartCoroutine(DashCooldown());
                SoundManager.Instance.PlaySFX("Dash");
            }
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
            StandUp();
        }
        if (crouching)
        {
            if (infiniteSlide && !crouchingMovment)
            {
                rb.velocity = new Vector3(lastVelocity.x, rb.velocity.y, lastVelocity.z);
                Vector3 moveVel = rb.velocity;
                if (rb.velocity.magnitude >= 0.5f)
                {
                    boostedCharge = true;
                }
            }
            if (crouchingMovment)
            {
                rb.AddForce(moveDirection * moveSpeed * 80f * Time.fixedDeltaTime);
                rb.AddForce(moveDirection * moveSpeed * 60f * Time.fixedDeltaTime);
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
        inputActions.PlayerActions.Look.canceled += LookStopped;
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
        inputActions.PlayerActions.Look.canceled -= LookStopped;
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        if (pauseMenu.activeSelf)
        {
            pauseMenu.GetComponent<PauseMenuBehavior>().EscapePressed();
        }
        else {
            pauseMenu.SetActive(true);
            pauseMenu.GetComponent<PauseMenuBehavior>().SelectFirstButton();
        }
    }

    private void LoadSensitivity()
    {
        rotationSpeed = PlayerPrefs.GetFloat("sens");
        sensSlider.value = rotationSpeed;
    }
}