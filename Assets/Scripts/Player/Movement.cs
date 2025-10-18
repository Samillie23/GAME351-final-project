using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private bool facingright = true;
    public bool isJumping = false;
    public bool isGrounded;
    private float moveDirection;

    [Header("Movement")]
    public float moveSpeed = 5;
    public float runSpeed = 7;
    private float currentSpeed;
    public float jumpForce = 80;
    public bool isRunning;

    [Header("Block")]
    public float blockSpeedMultiplier = 0.1f;     // <--- BLOCK EFFECT (slow movement)
    public bool blockStopsActions = true;          // <--- if true, can't roll/dash while blocking
    public bool isBlocking = false;                        // <--- BLOCK STATE

    [Header("Dodge / Roll")]
    public float rollSpeed = 11f;                  // <--- ROLL HORIZONTAL SPEED
    public float rollDuration = 0.35f;             // <--- ROLL TIME
    public float rollCooldown = 0.6f;              // <--- ROLL COOLDOWN
    public bool isRolling = false;                        // <--- ROLL STATE
    private float lastRollTime = -999f;            // <--- ROLL TIMER

    [Header("I-Frames")]
    public bool useIFrames = true;
    public int normalLayer = 0;
    public int iFrameLayer = 9;

    // (Optional) quick on-screen debug so teammates can see states
    [Header("Debug")]
    public bool debugUI = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
        if (useIFrames) gameObject.layer = normalLayer;
    }

    void Update()
    {
        GetInput();
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
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.1f, ground)) isGrounded = true;
        else isGrounded = false;
    }

    private void GetInput()
    {
        // move direction
        moveDirection = Input.GetAxis("Horizontal");
        transform.forward = new Vector3(moveDirection, 0, 0);

        // jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) isJumping = true;

        // block speed if true, run speed if true otherwise walk
        isBlocking = Input.GetKey(KeyCode.Mouse1);
        isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isBlocking ? moveSpeed * blockSpeedMultiplier : (isRunning ? runSpeed : moveSpeed);

        // rolling
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isRolling && Time.time >= lastRollTime + rollCooldown && isGrounded && (!blockStopsActions || !isBlocking))
        {
            StartCoroutine(RollCoroutine());       // <-- REQUIRED to start ROLL
        }
    }

    private void Move()
    {
        if (isGrounded && !isRolling) rb.velocity = new Vector3(moveDirection * currentSpeed, rb.velocity.y);
        if (isJumping) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = false;
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        lastRollTime = Time.time;

        if (anim) anim.SetTrigger("Roll");
        if (useIFrames) gameObject.layer = iFrameLayer;

        float t = 0f;

        while (t < rollDuration)
        {
            Vector3 v = rb.velocity;
            v.x = moveDirection * rollSpeed;                 // <-- ROLL movement
            rb.velocity = v;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (useIFrames) gameObject.layer = normalLayer;
        isRolling = false;
    }

    public IEnumerator TakingDamage(float hitStrength)
    {
        yield return new WaitForSeconds(.1f);
        rb.AddForce(-transform.forward * hitStrength, ForceMode.Impulse);
        rb.AddForce(transform.up * hitStrength, ForceMode.Impulse);
    }

    // Tiny on-screen debug so they can tell it’s working
    void OnGUI()
    {
        if (!debugUI) return;
        GUI.Label(new Rect(10, 10, 300, 20), $"Grounded: {isGrounded}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Blocking: {isBlocking}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Rolling:  {isRolling}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Dashing:  {isRunning}");
    }
}
