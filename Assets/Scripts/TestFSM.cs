using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestFSM : MonoBehaviour
{
    // possible states of the agent
    public enum StateType
    {
        Guard       = 0,
        Investigate = 1,
        Chase       = 2,
        Attack      = 3
    }

    // the speed multiplier for running
    public float speedMultiplier = 2.0f;

    public float maxAngle = 120.0f;

    // Agent AI
    NavMeshAgent agent;
    // animation controller
    Animator animController;

    // speed variables
    float walkSpeed;
    float runSpeed;

    // target objects
    GameObject target;
    GameObject player;

    // random orientation
    float rotAngle;

    // current active state
    int currState = (int) StateType.Guard;

    // get or set the current machine state
    public int State
    {
        get => currState;
        set
        {
            currState = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player;

        agent = GetComponent<NavMeshAgent>();

        walkSpeed = agent.speed;
        runSpeed  = agent.speed * speedMultiplier;

        //agent = GetComponent<NavMeshAgent>();
        animController = GetComponent<Animator>();

        // seed the animation offset and speed so that
        // every skeleton doesn't move exactly same way
        float randOffset = Random.Range(0.0f, 1.0f);
        float randSpeed  = Random.Range(0.25f, 3.0f);

        animController.SetFloat("Offset", randOffset);
        animController.SetFloat("Speed", randSpeed);

        animController.SetTrigger("isIdling");
    }

    // Update is called once per frame
    void Update()
    {
        int newState = State;
        // agent state toggle
        if (Input.GetKeyDown(KeyCode.T))
        {
            newState++;
            newState = newState % 3;

            State = newState;
        }

        // run the primitive state machine
        UpdateStateMachine();
        TransitionStates  ();
    }

    // a *very* primitive state machine implementation
    void UpdateStateMachine ()
    {
        animController.ResetTrigger("isIdling");
        animController.ResetTrigger("isAttacking");
        animController.ResetTrigger("isWalking");
        animController.ResetTrigger("isRunning");

        //Debug.Log("curr state is " + currState);

        // if there has been a change in active
        // state, run the relevant behaviors
        switch (currState)
        {
            case (int)StateType.Guard:
                Guard();
                break;

            case (int)StateType.Investigate:
                Investigate();
                break;

            case (int)StateType.Chase:
                Chase();
                break;

            case (int)StateType.Attack:
                Attack();
                break;
        }

    }

    // transition conditions for our primitive state machine
    void TransitionStates ()
    {
        switch (currState)
        {
            case (int)StateType.Guard:
                break;

            case (int)StateType.Investigate:
                // when agent has stopped moving, start guarding
                if (HasArrived() && !agent.isStopped)
                {
                    Guard();
                }
                break;

            case (int)StateType.Chase:
                // when agent has stopped moving, start attacking
                if (HasArrived() && !agent.isStopped)
                {
                    Attack();
                }
                break;

            case (int)StateType.Attack:
                // when player no longer in range, stop attacking
                Vector3 direction = player.transform.position - transform.position;
                if (direction.magnitude > agent.stoppingDistance)
                {
                    Guard();
                }
                break;
        }
    }

    void Guard ()
    {
        //Debug.Log("Entered Guard State");
        State = (int) StateType.Guard;

        animController.SetTrigger("isIdling");

        if (!agent.isStopped)
        {
            agent.isStopped = true;

            // find a random rotation angle to apply to agent
            rotAngle = GetAngle(transform.forward) + Random.Range (-1.0f, 1.0f) * maxAngle;
        }

        // slowly rotate into final guarding orientation
        float rotY = Mathf.LerpAngle(transform.rotation.eulerAngles.y, rotAngle,
                                     agent.angularSpeed * Time.deltaTime * 0.005f);

        transform.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);
    }

    void Investigate ()
    {
        //Debug.Log("Entered Investigate State");
        State = (int)StateType.Investigate;

        animController.SetTrigger("isWalking");
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(target.transform.position);
    }

    void Chase ()
    {
        //Debug.Log("Entered Chase State");
        State = (int)StateType.Chase;

        animController.SetTrigger("isRunning");

        target = player;
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(target.transform.position);
    }

    void Attack()
    {
        //Debug.Log("Entered Attack State");
        State = (int)StateType.Attack;

        animController.SetInteger("AttackID", Random.Range (0, 100));
        animController.SetTrigger("isAttacking");
        agent.isStopped = false;

        // face the player for the attack
        Vector3 direction = player.transform.position - transform.position;
        float angle = GetAngle (direction);

        float rotY  = Mathf.LerpAngle(transform.rotation.eulerAngles.y, angle,
                                      agent.angularSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler (0.0f, rotY, 0.0f);
    }

    // has the agent arrived at its target?
    bool HasArrived ()
    {
        //Debug.Log("Arrived at target" + target);
        Vector3 direction = agent.destination - transform.position;

        return (direction.magnitude < agent.stoppingDistance);
    }

    // get the Y-axis angle between two vectors (in degrees)
    float GetAngle (Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        return angle;
    }

    // a useful constrained random function
    float RandomBinomial()
    {
        return Random.value - Random.value;
    }
}
