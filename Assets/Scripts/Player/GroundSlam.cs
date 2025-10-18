using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GroundSlam : MonoBehaviour
{
    [Header("Slam Settings")]
    public float slamRadius;
    public float slamForce;
    private LayerMask attackableLayer;
    public KeyCode slamKey = KeyCode.J;

    private Animator animator;
    private bool isSlamming;
    private Movement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<Movement>();
        attackableLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        // Check for key press + jumping + not already slamming
        if (Input.GetKeyDown(slamKey) && !isSlamming && playerMovement.isJumping)
        {
            StartCoroutine(PerformGroundSlam());
        }
    }

    private IEnumerator PerformGroundSlam()
    {
        isSlamming = true;
        animator.SetTrigger("ground_slam");
        yield return new WaitForSeconds(0.5f);
        DoSlam();
        yield return new WaitForSeconds(0.5f);
        isSlamming = false;
    }

    private void DoSlam()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, slamRadius, attackableLayer);

        foreach (var hit in hits)
        {
            EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
            Rigidbody rb = hit.attachedRigidbody;

            if (enemy != null)
            {
                StartCoroutine(enemy.TakingDamage(slamForce));
            }
            else if (rb != null)
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                direction.y = 0.4f; 
                direction.z = 0f;   
                rb.AddForce(direction.normalized * slamForce, ForceMode.Impulse);
            }
        }
    }
}
