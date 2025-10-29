using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyAttackingNavMesh enemyAttacking;

    public bool attacking;

    void Start()
    {
        EnemyTracker.Instance.RegisterEnemy();
        agent = GetComponent<NavMeshAgent>();
        enemyAttacking = GetComponentInChildren<EnemyAttackingNavMesh>();
        attacking = false;
    }

    void OnDestroy()
    {
        if (EnemyTracker.Instance != null)
            EnemyTracker.Instance.UnregisterEnemy();
    }

    void Update()
    {
        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !attacking)
        {
            Debug.Log("Load cannon");
            enemyAttacking.Attack();
            attacking = true;
        }

        if (agent != null && agent.remainingDistance > agent.stoppingDistance && attacking)
        {
            enemyAttacking.CeaseFire();
            attacking = false;
        }
    }
}
