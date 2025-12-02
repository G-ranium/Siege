using UnityEngine;
using UnityEngine.Events;

public class NewTowerBehavior : MonoBehaviour, IDamageable
{
    public TowerData towerData;
    public UnityEvent onDeath;

    private HealthBarBehavior towerHealthbar;
    private float currentHealth;
    private float fireCooldown = 0f;
    private Transform currentTarget;

    private void Start()
    {
        if (towerData == null)
        {
            Debug.LogError("TowerData not assigned on " + gameObject.name);
            enabled = false;
            return;
        }

        currentHealth = towerData.maxHealth;
        towerHealthbar = gameObject.GetComponentInChildren<HealthBarBehavior>();
        towerHealthbar.UpdateHealthBar(currentHealth, towerData.maxHealth);
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        AcquireTarget();

        if (currentTarget != null)
        {
            RotateTowardTarget();
            HandleShooting();
        }
    }

    private void AcquireTarget()
    {
        // Try to keep current target if still valid
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            if (dist <= towerData.range) return;
        }

        // Otherwise find a new one
        Collider[] hits = Physics.OverlapSphere(transform.position, towerData.range, towerData.enemyLayer);

        currentTarget = hits.Length > 0 ? hits[0].transform : null;
    }

    private void RotateTowardTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0; // keep horizontal rotation

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }

    private void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / towerData.fireRate;
        }
    }

    private void Shoot()
    {
        if (currentTarget == null) return;
        IDamageable damageable = currentTarget.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(towerData.damage);
        }
        else
        {
            Debug.Log($"{currentTarget} doesn't have an IDamageable.");
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        towerHealthbar.UpdateHealthBar(currentHealth, towerData.maxHealth);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        onDeath.Invoke();
        // play death FX here
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (towerData == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }
}
