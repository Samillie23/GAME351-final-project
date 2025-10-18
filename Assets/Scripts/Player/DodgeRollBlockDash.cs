using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DodgeRollBlockDash : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private bool facingRight = true;
    private bool isGrounded;
    private bool isJumping;
    private float moveInput;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 7f;
    public float jumpForce = 400f;
    public float groundCheckDistance = 1.0f;
    private float currentSpeed;

    // ====== BLOCK (HOLD TO SLOW / PREVENT ACTIONS) ======
    [Header("Block")]
    public KeyCode blockKey = KeyCode.Mouse1;      // <--- BLOCK INPUT
    public float blockSpeedMultiplier = 0.35f;     // <--- BLOCK EFFECT (slow movement)
    public bool blockStopsActions = true;          // <--- if true, can't roll/dash while blocking
    private bool isBlocking;                        // <--- BLOCK STATE
    // ====== /BLOCK ======

    // ====== DODGE / ROLL (BURST WITH COOLDOWN) ======
    [Header("Dodge / Roll")]
    public KeyCode rollKey = KeyCode.LeftControl;  // <--- ROLL INPUT
    public float rollSpeed = 11f;                  // <--- ROLL HORIZONTAL SPEED
    public float rollDuration = 0.35f;             // <--- ROLL TIME
    public float rollCooldown = 0.6f;              // <--- ROLL COOLDOWN
    public bool airRoll = false;                   // <--- allow roll in air?
    private bool isRolling;                        // <--- ROLL STATE
    private float lastRollTime = -999f;            // <--- ROLL TIMER
    // ====== /DODGE / ROLL ======

    // ====== DASH (SHORTER, FASTER BURST) ======
    [Header("Dash")]
    public KeyCode dashKey = KeyCode.E;            // <--- DASH INPUT
    public float dashSpeed = 16f;                  // <--- DASH SPEED
    public float dashDuration = 0.18f;             // <--- DASH TIME
    public float dashCooldown = 0.5f;              // <--- DASH COOLDOWN
    public bool airDash = true;                    // <--- allow dash in air?
    private bool isDashing;                        // <--- DASH STATE
    private float lastDashTime = -999f;            // <--- DASH TIMER
    // ====== /DASH ======

    // (Optional) Temporary invulnerability during roll/dash via layer swap
    [Header("Optional i-Frames (layer swap)")]
    public bool useIFrames = false;
    public int normalLayer = 0;
    public int iFrameLayer = 8;

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
        ReadInput();
        HandleFlip();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        Move();
    }

    private void ReadInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            isJumping = true;

        // ====== BLOCK: read input + apply slow ======
        isBlocking = Input.GetKey(blockKey); // <-- REQUIRED for BLOCK
        currentSpeed = isBlocking ? moveSpeed * blockSpeedMultiplier   // <-- BLOCK effect
            : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed);
        // ====== /BLOCK ======

        // ====== DODGE / ROLL: input gate + cooldown ======
        if (Input.GetKeyDown(rollKey) &&
            !isRolling && !isDashing &&
            Time.time >= lastRollTime + rollCooldown &&
            (airRoll || isGrounded) &&
            (!blockStopsActions || !isBlocking))
        {
            StartCoroutine(RollCoroutine());       // <-- REQUIRED to start ROLL
        }
        // ====== /DODGE / ROLL ======

        // ====== DASH: input gate + cooldown ======
        if (Input.GetKeyDown(dashKey) &&
            !isRolling && !isDashing &&
            Time.time >= lastDashTime + dashCooldown &&
            (airDash || isGrounded) &&
            (!blockStopsActions || !isBlocking))
        {
            StartCoroutine(DashCoroutine());       // <-- REQUIRED to start DASH
        }
        // ====== /DASH ======
    }

    private void Move()
    {
        if (!isRolling && !isDashing)              // roll/dash override velocity
        {
            Vector3 v = rb.velocity;
            v.x = moveInput * currentSpeed;
            rb.velocity = v;
        }

        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce);
            isJumping = false;
        }
    }

    // ====== DODGE / ROLL IMPLEMENTATION ======
    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        lastRollTime = Time.time;

        FaceInputDirectionIfAny();

        if (anim) anim.SetTrigger("Roll");
        if (useIFrames) gameObject.layer = iFrameLayer;

        float t = 0f;
        float dir = RollDashDirection();

        while (t < rollDuration)
        {
            Vector3 v = rb.velocity;
            v.x = dir * rollSpeed;                 // <-- ROLL movement
            rb.velocity = v;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (useIFrames) gameObject.layer = normalLayer;
        isRolling = false;
    }
    // ====== /DODGE / ROLL IMPLEMENTATION ======

    // ====== DASH IMPLEMENTATION ======
    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        FaceInputDirectionIfAny();

        if (anim) anim.SetTrigger("Dash");
        if (useIFrames) gameObject.layer = iFrameLayer;

        float t = 0f;
        float dir = RollDashDirection();

        while (t < dashDuration)
        {
            Vector3 v = rb.velocity;
            v.x = dir * dashSpeed;                 // <-- DASH movement
            rb.velocity = v;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (useIFrames) gameObject.layer = normalLayer;
        isDashing = false;
    }
    // ====== /DASH IMPLEMENTATION ======

    private float RollDashDirection()
    {
        return (moveInput != 0f) ? Mathf.Sign(moveInput) : (facingRight ? 1f : -1f);
    }

    private void FaceInputDirectionIfAny()
    {
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();
    }

    private void HandleFlip()
    {
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateAnimator()
    {
        if (!anim) return;
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Blocking", isBlocking);  // <-- shows BLOCK state
        anim.SetBool("Rolling", isRolling);    // <-- shows ROLL state
        anim.SetBool("Dashing", isDashing);    // <-- shows DASH state
    }

    // Tiny on-screen debug so they can tell it’s working
    void OnGUI()
    {
        if (!debugUI) return;
        GUI.Label(new Rect(10, 10, 300, 20), $"Grounded: {isGrounded}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Blocking: {isBlocking}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Rolling:  {isRolling}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Dashing:  {isDashing}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
