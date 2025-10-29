using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackingNavMesh : MonoBehaviour
{
    private NavMeshAgent agent;
    private Projectile bullet;
    public int attackSpeed = 1;

    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        bullet = GetComponentInChildren<Projectile>();
    }

    public void SpawnProjectile()
    {
        if (agent != null && agent.destination != null)
        {
            transform.LookAt(agent.destination);
        }

        bullet.transform.position = transform.position;
        bullet.moving = true;
    }

    public void Attack()
    {
        InvokeRepeating(nameof(SpawnProjectile), attackSpeed, attackSpeed);
    }

    public void CeaseFire()
    {
        CancelInvoke(nameof(SpawnProjectile));
        bullet.moving = false;
    }
}
