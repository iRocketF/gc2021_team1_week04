using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 move;
    private Vector3 velocity;
    private bool isGrounded;

    public float debug;

    // Wall slide
    private bool isTouchingFront;
    public Transform frontCheck;
    private bool wallSliding;
    public float wallSlidingSpeed = -6;

    private float previousHeight;
    private bool isFalling;

    // Wall jump
    private bool wallJumping;
    public float xWallForce = 2f;
    public float yWallForce = 2.5f;
    public float wallJumpTime = 1f;

    [SerializeField] private Animator playerAnim;

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateAnimations();
    }

    void UpdateMovement()
    {
        // Ground and front checks
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isTouchingFront = Physics.CheckSphere(frontCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        

        //Rotation and movement of the character
        if (x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            move = transform.forward * x;
            move.z = 0;
        }   
        else if (x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
            move = -transform.forward * x;
            move.z = 0;
        }

        // Jumps
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerAnim.SetTrigger("jump");
            Invoke("Jump", 0.1f);
            //Jump();
        }

        CheckWallSliding(x);

        if (Input.GetButtonDown("Jump") && wallSliding)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }

        if (wallJumping)
        {
            move.x = xWallForce * -x;
            velocity.y = Mathf.Sqrt(yWallForce * -2f * gravity);
        }
        else if (wallSliding)
        {
            velocity.y += wallSlidingSpeed * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        previousHeight = transform.position.y;
        controller.Move((move * speed * Time.deltaTime) + velocity * Time.deltaTime);
    }

    // Checks whether the player is wall sliding or not
    void CheckWallSliding(float x)
    {
        isFalling = previousHeight > transform.position.y;

        if (isTouchingFront && !isGrounded && x != 0 && isFalling)
        {
            playerAnim.SetBool("isSliding", true);
            wallSliding = true;
        }
        else if (!isTouchingFront && !isGrounded && x != 0 && isFalling)
        {
            playerAnim.SetTrigger("fall");
            wallSliding = false;
        }
        else
        {
            playerAnim.SetBool("isSliding", false);
            wallSliding = false;
        }
    }

    private void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    void UpdateAnimations()
    {
        if (Input.GetAxis("Horizontal") != 0 && isGrounded)
        {
            playerAnim.SetBool("isLanding", true);
            playerAnim.SetBool("isRunning", true);
        }
        else if (isGrounded)
        {
            playerAnim.SetBool("isRunning", false);
            playerAnim.SetBool("isLanding", true);
        }

        if (!controller.isGrounded)
        {
            playerAnim.SetBool("isLanding", false);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            playerAnim.SetBool("isIdle", false);
        }
        else
        {
            playerAnim.SetBool("isIdle", true);
        }
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
