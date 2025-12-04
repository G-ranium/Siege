using UnityEngine;
using UnityEngine.AI;

public class MoveCenterNavMesh : MonoBehaviour
{
    public float range = 1.5f;  // Distance to stop
    public string baseName;

    [Header("Animation")]
    public Animator anim;
    public string moveAnim;
    public string idleAnim;

    public Transform defaultTarget; 
    public Transform currentTarget;

    private NavMeshAgent agent;
    private bool isMoving = false; // so we don't spam triggers

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = range;

        GameObject mainBase = GameObject.Find(baseName);
        defaultTarget = mainBase.transform;
        currentTarget = defaultTarget;

        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            PlayMoveAnim();   // Start moving immediately
        }
        else
        {
            PlayIdleAnim();
        }
    }

    void Update()
    {
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);

            // Check if close enough to stop
            if (!agent.pathPending && agent.remainingDistance <= range)
            {
                agent.isStopped = true;
                PlayIdleAnim(); // Stopped moving
            }
            else
            {
                agent.isStopped = false;
                PlayMoveAnim(); // Moving
            }
        }
        else 
        {
            // No target → try fallback
            if (defaultTarget != null)
            {
                currentTarget = defaultTarget;
                agent.isStopped = false;
                agent.SetDestination(defaultTarget.position);
                PlayMoveAnim();
            }
            else if (agent.remainingDistance < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        currentTarget = other.transform;
        PlayMoveAnim();
        //Debug.Log("Found Hostile!");
    }

    //-------------------------------
    // ANIMATION CONTROL (clean + reusable)
    //-------------------------------
    void PlayMoveAnim()
    {
        if (!isMoving)
        {
            anim.SetTrigger(moveAnim);
            isMoving = true;
        }
    }

    void PlayIdleAnim()
    {
        if (isMoving)
        {
            anim.SetTrigger(idleAnim);
            isMoving = false;
        }
    }
}
