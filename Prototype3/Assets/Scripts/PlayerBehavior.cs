using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    private PlayerControls inputActions;
    private Vector2 moveInput;
    private Vector2 lookDelta;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float verticalRotation = 0f;

    public GameObject eyes; //camera
    public Slider jumpChargeMeter;
    public float maxLookAngle = 80f;
    public float jumpForce = 10f;
    public float crouchChargeTime = 2f;
    public float maxCrouchJumpPower = 10f;
    public float moveSpeed = 5f;
    public float airMoveSpeed = 2.5f;
    
    public float rotationSpeed = 5f;
    public float dashPower = 100f;
    public float groundDashCooldown = 1f;
    [Tooltip("Set between 0-1")]
    public float groundDashReduction = 0.75f;

    private bool crouching = false;
    private bool crouchingMovment = false;
    private bool capsLockHeld = false;
    private float crouchStartTime;

    private PlayerAbilities playerAbilities;
    private bool canDashGround = true;
    private bool canDashAir = true;
    private float chargeStrength;
    private bool paused;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        playerAbilities = GetComponent<PlayerAbilities>();
        paused = false;
    }
    private void FixedUpdate()
    {
        MovePlayer();
        UpdateJumpChargeSlider();
    }
    private void UpdateJumpChargeSlider() {
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
        if (!crouching ) {
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
        if (crouching && !crouchingMovment) {
            StandUp();
        }
    }
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        capsLockHeld = true;
        crouchStartTime = Time.time;
        if (grounded()) {
            capsuleCollider.height = 1;
            capsuleCollider.center = new Vector3(0,-0.5f, 0);
            crouching = true;
            Vector3 newPos = transform.position;
            newPos.y -= 0.5f;
            eyes.transform.position = newPos;
        }
    }
    private void CrouchCancled(InputAction.CallbackContext context)
    {
        capsLockHeld = false;
        if (crouching && CanUncrouch()) {
            StandUp();
        }  
        else if (crouching)
        {
            crouchingMovment = true;
        }
    }
    private void StandUp() {
        capsuleCollider.height = 2;
        capsuleCollider.center = Vector3.zero;
        crouching = false;
        crouchingMovment = false;
        Vector3 newPos = transform.position;
        newPos.y += 0.5f;
        eyes.transform.position = newPos;
        rb.velocity = Vector2.zero;
    }
    private void OnDash(InputAction.CallbackContext ctx)
    {
        if(grounded() && canDashGround)
        {
            playerAbilities.Dash(dashPower * groundDashReduction);
            canDashGround = false;
            StartCoroutine(GroundCooldown());
        }
        else if(!grounded() && canDashAir)
        {
            playerAbilities.Dash(dashPower);
            canDashAir = false;
        }
    }
    private bool CanUncrouch() {
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
    private bool grounded() {
        float rayDistance = 0.6f;
        Vector3 BoxSize = new Vector3(0.4f, 0.5f, 0.4f);
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        RaycastHit[] hits = Physics.BoxCastAll(centerPos, BoxSize, Vector3.down, Quaternion.identity, rayDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                canDashAir = true;
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
        if (crouching) {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
            if (flatVelocity.magnitude > moveSpeed)
            {
                rb.velocity = new Vector3(flatVelocity.normalized.x * moveSpeed, rb.velocity.y, flatVelocity.normalized.z * moveSpeed);
            }
            if (crouchingMovment) {
                rb.AddForce(moveDirection * moveSpeed * 40f * Time.fixedDeltaTime);
            }
        } else {
            
            if (grounded()) {
                rb.AddForce(moveDirection * moveSpeed * 100f * Time.fixedDeltaTime);
            } else {
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

    /// <summary>
    /// Pauses the game
    /// </summary>
    /// <param name="obj"></param>
    private void Pause_started(InputAction.CallbackContext obj)
    {
        paused = !paused;
        Cursor.visible = paused;
        if(paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
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

    private IEnumerator GroundCooldown()
    {
        yield return new WaitForSeconds(groundDashCooldown);

        canDashGround = true;
    }
}
