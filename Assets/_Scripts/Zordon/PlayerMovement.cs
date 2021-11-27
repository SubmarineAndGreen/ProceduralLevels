using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class PlayerMovement : MonoBehaviour
{

    private float playerHeight=2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float moveMultiplier = 6f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpTime;
    [SerializeField] private float timeBeforeJetpack;
    private bool isJumping;

    [Header("Jetpack")]
    [SerializeField] private GameObject jetpackVFX;
    [SerializeField] private float jetpackForce = 5f;
    [SerializeField] private float jetpackDuration = 10;
    private float jetpackUsedTime;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private float verticalMove;
    private float horizontalMove;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    private bool isGrounded;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    //na ziemi pod k¹tem
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isJumping = false;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //print(isGrounded);
        MyInput();
        ControlDrag();

        //zapewnienie prawie maksymalnego skoku przed uruchomieniem jetpacka
        if (jumpTime < Time.fixedTime) isJumping = false;
        //skok
        if (inputs.Player.Jump.triggered && isGrounded)
        {
            jumpTime = Time.fixedTime + timeBeforeJetpack;
            isJumping = true;
            Jump();
        }
        //resetuj jetpack, jeœli na ziemi
        if (isGrounded && jetpackUsedTime != 0)
        {
            if (jetpackUsedTime < 2) jetpackUsedTime = 0;
            jetpackUsedTime -= 2;
        }
        //jeœli w powietrzu, zamiast skoku u¿yj jetpacka i zwiêksz zu¿ycie siê jetpacka
        if (inputs.Player.Jump.ReadValue<float>()==1 && !isGrounded && !isJumping && jetpackUsedTime<jetpackDuration)
        {
            var jetpack = Instantiate(jetpackVFX, groundCheck.position, Quaternion.identity) as GameObject;
            Destroy(jetpack, 0.1f);
            jetpackUsedTime +=Time.deltaTime;
            Jetpack();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    
    private void Jetpack()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jetpackForce, ForceMode.Acceleration);
    }
    
    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    void MyInput()
    {
        var input = inputs.Player.MovementAxes.ReadValue<Vector2>();
        Debug.Log(input);
        horizontalMove = input.x;
        verticalMove = input.y;

        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;
    }

    //zmieñ opory jeœli na ziemi lub powietrzu
    private void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        //grawitacja zale¿na od masy, niezale¿na od oporów powietrza
        //rb.AddForce(Physics.gravity * rb.mass,ForceMode.Acceleration);
    }
    
    void MovePlayer()
    {
        //na ziemi
        if(isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);

        }
        //na ziemi pod k¹tem
        else if(isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        //w powietrzu
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier *airMultiplier, ForceMode.Acceleration);
        }
    }
}
