using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    GameObject player;
    GameObject target;
    Animator animator;
    NavMeshAgent navMeshAgent;

    [SerializeField] bool canSeePlayer()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance < 8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [SerializeField] bool canAttackPlayer()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance < 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // possible states of the agent
    public enum StateType
    {
        Idle,
        Chase,
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
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
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

            case (int)StateType.Attack:
                Attack();
                break;
        }

    }

    void TransitionStates ()
    {
        switch (currState)
        {
            case (int)StateType.Idle:
                break;

            case (int)StateType.Chase:
                if (canAttackPlayer())
                {
                    Attack();
                }
                break;

            case (int)StateType.Attack:
                if (!canAttackPlayer())
                {
                    Idle();
                }
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

    void Attack()
    {
        State = (int)StateType.Attack;

        navMeshAgent.isStopped = true;
    }
}
