using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private bool facingright = true;
    private bool isJumping = false;
    private bool isGrounded;
    private float moveDirection;

    public float moveSpeed = 5;
    public float runSpeed = 7;
    private float currentSpeed;
    public float jumpforce = 400;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        GetInput();
        // animate
        Animate();
    }

    void FixedUpdate()
    {
        // check ground
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1f);
        
        // move
        Move();
    }

    private void GetInput()
    {
        // movement
        moveDirection = Input.GetAxis("Horizontal");
        // jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jumping");
            isJumping = true;
        }
        // sprinting
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
            rb.velocity = new Vector3(moveDirection * currentSpeed, rb.velocity.y);
        }
        if (isJumping)
        {
            rb.AddForce(new Vector3(0f, jumpforce, 0f));
        }
        isJumping = false;
    }


    private void Animate()
    {
        if (moveDirection > 0 && !facingright)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && !facingright)
        {
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        facingright = !facingright;
        transform.Rotate(0f, 180f, 0f);
    }
}
