using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DodgeRollBlockDash : MonoBehaviour{
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

    [Header("Block")]
    public KeyCode blockKey = KeyCode.Mouse1;
    public float blockSpeedMultiplier = 0.35f;
    public bool blockStopsActions = true;
    private bool isBlocking;

    [Header("Dodge / Roll")]
    public KeyCode rollKey = KeyCode.LeftControl;
    public float rollSpeed = 11f;
    public float rollDuration = 0.35f;
    public float rollCooldown = 0.6f;
    public bool airRoll = false;
    private bool isRolling;
    private float lastRollTime = -999f;

    [Header("Dash")]
    public KeyCode dashKey = KeyCode.E;
    public float dashSpeed = 16f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.5f;
    public bool airDash = true;
    private bool isDashing;
    private float lastDashTime = -999f;

    [Header("Optional i-Frames (layer swap)")]
    public bool useIFrames = false;
    public int normalLayer = 0;
    public int iFrameLayer = 8;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
        if(useIFrames){gameObject.layer = normalLayer;}      
    }

    void Update(){
        ReadInput();
        HandleFlip();
        UpdateAnimator();  
    }

    void FixedUpdate(){
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        Move();
    }

    private void ReadInput(){
        moveInput = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            isJumping = true;
        }

        isBlocking = Input.GetKey(blockKey);
        if(isBlocking){
            currentSpeed = moveSpeed * blockSpeedMultiplier;
        }

        else{
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
        }

        if(Input.GetKeyDown(rollKey) && !isRolling&& !isDashing && Time.time >= lastRollTime + rollCooldown && (airRoll || isGrounded) && (!blockStopsActions || !isBlocking)){
            StartCoroutine(RollCoroutine());
        }

        if(Input.GetKeyDown(dashKey) && !isRolling&& !isDashing && Time.time >= lastDashTime + dashCooldown && (airDash || isGrounded) && (!blockStopsActions || !isBlocking)){
            StartCoroutine(DashCoroutine());
        }

    }

    private void Move(){
        if(!isRolling && !isDashing){
            Vector3 v = rb.velocity;
            v.x = moveInput * currentSpeed;
            rb.velocity = v;
        }

        if(isJumping){
            rb.AddForce(Vector3.up * jumpForce);
            isJumping = false;
        }
    }

    private IEnumerator RollCoroutine(){
        isRolling = true;
        lastRollTime = Time.time;

        FaceInputDirectionIfAny();

        if(anim){anim.SetTrigger("Roll");}
        if(useIFrames){gameObject.layer = iFrameLayer;}

        float t = 0f;
        float dir = RollDashDirection();

        while(t < rollDuration){
            Vector3 v = rb.velocity;
            v.x = dir * rollSpeed;
            rb.velocity = v;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(useIFrames){
            gameObject.layer = normalLayer;
        }

        isRolling = false;

    }

    private IEnumerator DashCoroutine(){
        isDashing = true;
        lastDashTime = Time.time;

        FaceInputDirectionIfAny();

        if(anim){anim.SetTrigger("Dash");}
        if(useIFrames){gameObject.layer = iFrameLayer;}

        float t = 0f;
        float dir = RollDashDirection();

        while(t < dashDuration){
            Vector3 v = rb.velocity;
            v.x = dir * dashSpeed;
            rb.velocity = v;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(useIFrames){
            gameObject.layer = normalLayer;
        }

        isDashing = false;

    }

    private float RollDashDirection(){
        return (moveInput != 0f) ? Mathf.Sign(moveInput) : (facingRight ? 1f : -1f);
    }

    private void FaceInputDirectionIfAny(){
        if(moveInput > 0 && !facingRight){Flip();}
        else if(moveInput < 0 && facingRight){Flip();}
    }

    private void Flip(){
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateAnimator(){
        if(!anim) {return;}
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Blocking", isBlocking);
        anim.SetBool("Rolling", isRolling);
        anim.SetBool("Dashing", isDashing);
    }

    void OnDrawGizmosSelected(){
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

}
