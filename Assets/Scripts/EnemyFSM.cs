using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    GameObject player;
    GameObject target;
    Animator animator;
    NavMeshAgent navMeshAgent;
    Rigidbody rb;

    private bool isAttacking = false;
    private bool isHit;
    private float attackRadius = 2.5f;
    private float attackForce = 40f;
    private LayerMask attackableLayer;

    [SerializeField]
    bool canSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return (distance < 10) ? true : false;
    }
    
    [SerializeField]
    bool canAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return (distance < 3) ? true : false;
    }
    
    float GetAngle (Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        return angle;
    }

    // possible states of the agent
    public enum StateType
    {
        Idle,
        Chase,
        Attacking,
        Attack
    }

    // current active state
    [SerializeField] int currState = (int)StateType.Idle;

    // get or set the current machine state
    public int State
    {
        get => currState;
        set
        {
            currState = value;
        }
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        attackableLayer.value = 1 << LayerMask.GetMask("Player");
    }

    void Update()
    {
        navMeshAgent.enabled = isHit ? false : true;             
        // run the primitive state machine
        UpdateStateMachine();
        TransitionStates  ();
    }

    void UpdateStateMachine ()
    {
        // if there has been a change in active
        // state, run the relevant behaviors
        switch (currState)
        {
            case (int)StateType.Idle:
                Idle();
                break;

            case (int)StateType.Chase:
                Chase();
                break;

            case (int)StateType.Attacking:
                Attacking();
                break;
        }

    }

    void TransitionStates ()
    {
        switch (currState)
        {
            case (int)StateType.Idle:
                if (canSeePlayer()) Chase();
                break;

            case (int)StateType.Chase:
                if (canAttackPlayer()) Attacking();
                break;

            case (int)StateType.Attack:
                if (!canAttackPlayer()) Idle();
                break;
        }
    }

    void Idle()
    {
        State = (int)StateType.Idle;

        if (!navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = true;
        }
    }

    void Chase()
    {
        State = (int)StateType.Chase;
        target = player;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.transform.position);
    }

    void Attacking()
    {
        State = (int)StateType.Attack;

        // face player
        Vector3 direction = player.transform.position - transform.position;
        float angle = GetAngle(direction);
        float rotY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, angle, navMeshAgent.angularSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);

        navMeshAgent.isStopped = true;
        StartCoroutine(CheckAttack());
    }
    
    IEnumerator CheckAttack()
    {
        while (canAttackPlayer())
        {
            yield return new WaitForSeconds(1f);
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(ApplyForce());
            }
        }
    }

    IEnumerator ApplyForce()
    {
        yield return new WaitForSeconds(.25f);
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, attackRadius, attackableLayer.value);

        foreach (var hit in hits)
        {
            Movement player = hit.GetComponentInParent<Movement>();
            if (player != null)
            {
                StartCoroutine(player.TakingDamage(attackForce));
            }
        }
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }

    public IEnumerator TakingDamage(float hitStrength)
    {
        isHit = true;
        yield return new WaitForSeconds(.1f);
        rb.AddForce(-transform.forward * hitStrength, ForceMode.Impulse);
        rb.AddForce(transform.up * hitStrength, ForceMode.Impulse);
        yield return new WaitForSeconds(2.3f);
        isHit = false;
    }
}
