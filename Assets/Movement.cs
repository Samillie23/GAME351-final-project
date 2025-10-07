using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private bool facingright = true;
    private bool isJumping = false;
    private float moveDirection;
    public float moveSpeed;
    public float jumpforce;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        // move
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector3(moveDirection * moveSpeed, rb.velocity.y);
        if (isJumping)
        {
            rb.AddForce(new Vector3(0f, jumpforce, 0f));
        }
        isJumping = false;
    }

    private void GetInput()
    {
        moveDirection = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jumping");
            isJumping = true;
        }
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
