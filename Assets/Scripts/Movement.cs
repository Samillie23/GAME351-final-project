using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private bool facingright = true;
    private bool isJumping = false;
    [SerializeField] private bool isGrounded;
    private float moveDirection;

    public float moveSpeed = 5;
    public float runSpeed = 7;
    private float currentSpeed;
    public float jumpForce = 400;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        GetInput();
        Animate();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    void CheckGround()
    {
        LayerMask ground = LayerMask.GetMask("Ground");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.1f, ground))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void GetInput()
    {
        moveDirection = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = moveSpeed;
        }
    }

    private void Move()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);
        }

        if (isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }

        isJumping = false;
    }

    private void Animate()
    {
        if (moveDirection > 0 && !facingright)
        {
            facingright = !facingright;
            transform.Rotate(0f, 180f, 0f);
        }
        else if (moveDirection < 0 && !facingright)
        {
            facingright = !facingright;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
