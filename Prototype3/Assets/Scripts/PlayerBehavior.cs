using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    private PlayerControls inputActions;
    private Vector2 moveInput;
    private Vector2 lookDelta;
    private Rigidbody rb;
    private float verticalRotation = 0f;

    public GameObject eyes; //camera
    public float maxLookAngle = 80f;
    public float jumpForce = 10f;
    public float crouchChargeTime = 2f;
    public float maxCrouchJumpPower = 10f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float crouchSpeed = 1f;

    public bool crouching;
    private float crouchStartTime;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions = new PlayerControls();
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
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
        if (grounded() && crouching)
        {
            float chargeStrength = (Time.time - crouchStartTime) / crouchChargeTime;
            if (chargeStrength > 1) {
                chargeStrength = 1;
            }
            rb.AddForce(Vector3.up * (jumpForce + (maxCrouchJumpPower * chargeStrength)), ForceMode.Impulse);
        }
        else if (grounded()) 
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        if (crouching) {
            crouching = false;
            Vector3 newPos = transform.position;
            newPos.y += 0.5f;
            eyes.transform.position = newPos;
        }
    }
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        crouchStartTime = Time.time;
        if (grounded()) {
            crouching = true;
            Vector3 newPos = transform.position;
            newPos.y -= 0.5f;
            eyes.transform.position = newPos;
        }
    }
    private void CrouchCancled(InputAction.CallbackContext context)
    {
        if (crouching) {
            crouching = false;
            Vector3 newPos = transform.position;
            newPos.y += 0.5f;
            eyes.transform.position = newPos;
            rb.velocity = Vector2.zero;
        }  
    }
    private bool grounded() {
        float rayDistance = 1f;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, rayDistance);
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

        if (crouching) {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
            if (flatVelocity.magnitude > moveSpeed)
            {
                rb.velocity = new Vector3(flatVelocity.normalized.x * moveSpeed, rb.velocity.y, flatVelocity.normalized.z * moveSpeed);
            }
        } else {
            
            if (grounded()) {
                rb.AddForce(moveDirection * moveSpeed * 100f * Time.fixedDeltaTime);
            } else {
                rb.AddForce(moveDirection * moveSpeed * 20f * Time.fixedDeltaTime);
            } 
            Vector3 flatVelocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
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
    }
}
