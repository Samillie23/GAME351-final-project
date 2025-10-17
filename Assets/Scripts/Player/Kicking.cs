using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicking : MonoBehaviour
{
    public float KickForce = 10f;
    public float KickRadius = 2f;
    public LayerMask KickableLayer;

    private Animator animator;
    private bool isKicking;
    private readonly string[] kicks = { "kick1", "kick2", "kick3" };

    void Start()
    {
        animator = GetComponent<Animator>();
        KickableLayer = LayerMask.GetMask("Attackable");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isKicking)
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
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, KickRadius, KickableLayer);

        foreach (var hit in hits)
        {
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                StartCoroutine(enemy.TakingDamage(KickForce));
            }
        }
    }

    void ResetKick()
    {
        isKicking = false;
    }
}
