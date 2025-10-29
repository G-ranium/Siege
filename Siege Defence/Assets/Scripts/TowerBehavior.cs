using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class TowerBehavior : MonoBehaviour, IDamageable
{
    [Header("Tower Configuration")]
    public TowerData towerData;
    
    private HealthBarBehavior towerHealthbar;
    private float currentHealth;
    private float fireCooldown;

    private SphereCollider rangeCollider;
    private Animator animator;

    // Targeting
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private GameObject currentTarget;

    private void Start()
    {
        if (towerData == null)
        {
            Debug.LogError("TowerData is not assigned on " + gameObject.name);
            return;
        }

        InitializeTower();
    }

    private void Update()
    {
        if (towerData == null) return;

        HandleTargeting();
        HandleFiring();
    }

    private void InitializeTower()
    {
        currentHealth = towerData.health;
        fireCooldown = 0f;
        towerHealthbar = gameObject.GetComponentInChildren<HealthBarBehavior>();

        // Set up range collider
        rangeCollider = GetComponent<SphereCollider>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<SphereCollider>();
        }
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned on " + gameObject.name);
        }
        animator.SetTrigger("Idle");

        rangeCollider.isTrigger = true;
        rangeCollider.radius = towerData.range;
    }

    private void HandleFiring()
    {
        if (currentTarget == null) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            FireAt(currentTarget);
            animator.SetTrigger("Fire");
            fireCooldown = 1f / towerData.fireRate;
            animator.SetTrigger("Reload");
        }
    }

    private void FireAt(GameObject enemy)
    {
        if (enemy == null) return;

        // You'd normally call something like enemy.TakeDamage(towerData.damage);
        Debug.Log($"{towerData.towerName} fires at {enemy.name} for {towerData.damage} damage.");
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(towerData.damage);
        }
        else
        {
            Debug.Log($"{towerData.towerName} doesn't have an IDamageable.");
        }
    }

    private void HandleTargeting()
    {
        // Clean up null enemies (e.g., destroyed mid-way)
        enemiesInRange.RemoveAll(enemy => enemy == null);

        // If current target is gone or out of range, switch to next
        if (currentTarget == null || !enemiesInRange.Contains(currentTarget))
        {
            currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
            Vector3 direction = currentTarget.transform.position - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
                Debug.Log($"{other.name} entered range of {towerData.towerName}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemiesInRange.Contains(other.gameObject))
            {
                Debug.Log($"{other.name} exited range of {towerData.towerName}");
                enemiesInRange.Remove(other.gameObject);

                // If it was the current target, reset targeting
                if (currentTarget == other.gameObject)
                {
                    currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage from {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
        currentHealth -= amount;
        towerHealthbar.UpdateHealthBar(currentHealth, towerData.health);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{towerData.towerName} has been destroyed.");
        Destroy(gameObject);
    }

    public void UpgradeTower(TowerData newData)
    {
        towerData = newData;
        InitializeTower();
    }

    private void OnDrawGizmosSelected()
    {
        if (towerData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, towerData.range);
        }
    }
}
