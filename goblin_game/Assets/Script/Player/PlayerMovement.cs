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

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");

        if (x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            move = transform.forward * x;
        }   
        else if (x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
            move = -transform.forward * x;
        }
            
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move((move * speed * Time.deltaTime) + velocity * Time.deltaTime);
    }
}
