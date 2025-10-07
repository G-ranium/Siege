using UnityEngine;

public class EnemyAttacking : MonoBehaviour
{
    private MoveCenter moveCenter;

    public GameObject bulletShape;
    public int attackSpeed;

    private void Start()
    {
        moveCenter = GetComponentInParent<MoveCenter>();
    }

    public void SpawnProjectile()
    {
        transform.LookAt(moveCenter.destination);
        Vector3 spawnLocation = transform.position;
        Projectile proj = Instantiate(bulletShape, spawnLocation, transform.rotation).GetComponent<Projectile>();
        proj.owner = gameObject;
    }

    public void Attack()
    {
        InvokeRepeating("SpawnProjectile", attackSpeed, attackSpeed);
        Debug.Log("Attacking");
    }

    public void CeaseFire()
    {
        CancelInvoke("SpawnProjectile");
        Debug.Log("Moving On");
    }
}
