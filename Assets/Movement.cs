using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    public Transform playerCamera;
    private bool readyToJump;
    private bool grounded;
    public Transform groundChecker;
    public LayerMask whatIsGround;
    private Rigidbody rb;
    public float moveSpeed = 10f;
    public float jumpCooldown = 1f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
    }
   
    void Update()
    {
        Look();
        Move();
    }

    private void Move()
    {
        grounded = Physics.OverlapSphere(groundChecker.position, 0.1f, whatIsGround).Length > 0;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        bool jumping = Input.GetButton("Jump");

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(x,y,mag);

        if (x > 0 && xMag > 5) x = 0;
        if (x < 0 && xMag < -5) x = 0;
        if (y > 0 && yMag > 5) y = 0;
        if (y < 0 && yMag < 5) y = 0;

        rb.AddForce(transform.forward * y * moveSpeed * Time.deltaTime);
        rb.AddForce(transform.right * x * moveSpeed * Time.deltaTime);

        if(grounded && readyToJump && jumping)
        {
            readyToJump = false;
            rb.AddForce(Vector3.up * 200);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded) return;
        //float threshold = 0.3f;
        float multiplier = 0.3f;

        if (x == 0)
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * multiplier);

        }
        if (y == 0)
        {
            rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * multiplier);

        }
    }
    private Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
}
