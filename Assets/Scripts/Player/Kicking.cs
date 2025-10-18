using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicking : MonoBehaviour
{
    public float kickForce;
    public float kickRadius;
    private LayerMask attackableLayer;
    public KeyCode kickKey = KeyCode.P;

    private Animator animator;
    private bool isKicking;
    private readonly string[] kicks = { "kick1", "kick2", "kick3" };

    void Start()
    {
        animator = GetComponent<Animator>();
        attackableLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        if (Input.GetKeyDown(kickKey) && !isKicking)
            Kick();
    }

    void Kick()
    {
        isKicking = true;
        animator.SetTrigger(kicks[Random.Range(0, kicks.Length)]);
        Invoke(nameof(ApplyForce), 0.25f);
        Invoke(nameof(ResetKick), 0.5f);
    }

    void ApplyForce()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, kickRadius, attackableLayer);

        foreach (var hit in hits)
        {
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                StartCoroutine(enemy.TakingDamage(kickForce));
            }
        }
    }

    void ResetKick()
    {
        isKicking = false;
    }
}
