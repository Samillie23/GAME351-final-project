using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Punching : MonoBehaviour
{
    [Header("Attack Settings")]
    public float punchForce;
    public float uppercutForce;
    public float punchRadius;
    public float comboResetTime;
    private LayerMask attackableLayer;
    public KeyCode punchKey = KeyCode.O;

    private Animator animator;
    private int punchIndex = 0;
    private bool isPunching = false;
    private float comboTimer = 0f;

    private readonly string[] punches = { "jab1", "jab2", "uppercut" };

    void Start()
    {
        animator = GetComponent<Animator>();
        attackableLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        comboTimer += Time.deltaTime;

        if (Input.GetKeyDown(punchKey) && !isPunching)
        {
            if (comboTimer > comboResetTime)
                punchIndex = 0; // Reset combo

            Punch();
            comboTimer = 0f;
        }
    }

    void Punch()
    {
        isPunching = true;
        string currentPunch = punches[punchIndex];
        animator.SetTrigger(currentPunch);
        float delay = currentPunch == "uppercut" ? 0.4f : 0.25f;
        Invoke(nameof(ApplyForce), delay);
        Invoke(nameof(ResetPunch), 0.6f);
        punchIndex = (punchIndex + 1) % punches.Length;
    }

    void ApplyForce()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, punchRadius, attackableLayer);

        foreach (var hit in hits)
        {
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                if (punchIndex == 0 || punchIndex == 1)
                {
                    // Regular jab
                    StartCoroutine(enemy.TakingDamage(punchForce));
                }
                else
                {
                    // Uppercut: launch enemy upward
                    StartCoroutine(enemy.TakingDamage(uppercutForce));
                }
            }
        }
    }

    void ResetPunch()
    {
        isPunching = false;
    }
}
