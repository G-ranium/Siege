using UnityEngine;
using UnityEngine.AI;

public class MoveCenterNavMesh : MonoBehaviour
{
    public float range = 1.5f; // Distance to stop
    public Transform defaultTarget; // Fallback target (e.g., center beacon)
    public Transform currentTarget;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = range;

        GameObject mainBase = GameObject.Find("MainBase");
        defaultTarget = mainBase.transform;
        currentTarget = defaultTarget;

        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    void Update()
    {
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);

            if (!agent.pathPending && agent.remainingDistance <= range)
            {
                agent.isStopped = true;
                // Optional: trigger attack or animation here
            }
        }
        else if (currentTarget == null && agent.destination != defaultTarget.position)
        {
            // No target — return to default or self-destruct
            if (defaultTarget != null)
            {
                currentTarget = defaultTarget;
                agent.isStopped = false;
                agent.SetDestination(defaultTarget.position);
                Debug.Log("Moving On");
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
        Debug.Log("Found Hostile!");
    }
}
