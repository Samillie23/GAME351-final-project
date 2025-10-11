using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    GameObject player;
    Animator animator;
    NavMeshAgent navMeshAgent;

    bool canSeePlayer()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance < 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool canAttackPlayer()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance < 3)
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
    int currState = (int)StateType.Idle;

    // get or set the current machine state
    public int State
    {
        get => currState;
        set
        {
            currState = value;
        }
    }

    void Start()
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
        Debug.Log("curr state is " + currState);

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
                if (canSeePlayer())
                {
                    Chase();
                }
                break;

            case (int)StateType.Chase:
                if (canAttackPlayer())
                {
                    Attack();
                }
                else if (!canSeePlayer())
                {
                    Idle();
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

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.transform.position);
        //animController.SetTrigger("isRunning");

        //target = player;
        //agent.isStopped = false;
        //agent.speed = runSpeed;
        //agent.SetDestination(target.transform.position);
    }

    void Attack()
    {
        State = (int)StateType.Attack;

        navMeshAgent.isStopped = false;
        //animController.SetInteger("AttackID", Random.Range (0, 100));
        //animController.SetTrigger("isAttacking");
        //agent.isStopped = false;

        // face the player for the attack
        //Vector3 direction = player.transform.position - transform.position;
        //float angle = GetAngle (direction);

        //float rotY  = Mathf.LerpAngle(transform.rotation.eulerAngles.y, angle, agent.angularSpeed * Time.deltaTime);

        //transform.rotation = Quaternion.Euler (0.0f, rotY, 0.0f);
    }
}
