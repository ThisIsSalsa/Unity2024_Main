using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeedMultiplier = 2f; // Multiplier for sprint speed
    private float currentMoveSpeed; // Variable to store current movement speed
    public float groundDrag;

    [Header("Ground Check")]
    public float playerWeight; 
    public LayerMask whatIsGround;
    public Transform raycastOrigin; 
    bool grounded;
    public Transform orientation;
    float horizontalInput; 
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public float maxAirborneSpeed; // Maximum move speed when airborne


    [Header("Jump")]
    public float jumpForce = 500f; // Force applied when jumping
    bool canJump = true; // Flag to prevent multiple jumps

    private void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentMoveSpeed = moveSpeed; // Initialize current movement speed
    }

    private void MyInputs(){
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        verticalInput = Input.GetAxisRaw("Vertical");
    }

   private bool wasGrounded = true; // Track previous grounded state

private void Update(){
    MyInputs();
    // Use raycastOrigin instead of transform.position
    grounded = Physics.Raycast(raycastOrigin.position, Vector3.down, playerWeight * 0.2f, whatIsGround); 
    Debug.DrawRay(raycastOrigin.position, Vector3.down * (playerWeight * 0.2f), Color.red);

    if (grounded) {
        rb.drag = groundDrag;
        canJump = true; // Reset jump flag when grounded
        currentMoveSpeed = moveSpeed; // Set move speed to default value when grounded
    } else {
        rb.drag = 0;
        currentMoveSpeed = Mathf.Min(moveSpeed + 1, maxAirborneSpeed); // Set move speed to a capped value when not grounded
    }

    // Sprint when Shift is pressed
    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
        currentMoveSpeed = moveSpeed * sprintSpeedMultiplier; // Adjust current speed for sprinting
    }

    // Cap currentMoveSpeed to maxAirborneSpeed
    if (!grounded && currentMoveSpeed > maxAirborneSpeed) {
        currentMoveSpeed = maxAirborneSpeed;
    }

    // Check for jump input
    if (Input.GetButtonDown("Jump") && grounded && canJump) {
        rb.AddForce(Vector3.up * jumpForce);
        canJump = false; // Set flag to prevent multiple jumps until grounded again
    }
}

    private void FixedUpdate(){
        MovePlayer();
    }

    private void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force); 
    }
}
