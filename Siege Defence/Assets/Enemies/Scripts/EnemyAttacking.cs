using UnityEngine;

public class EnemyAttacking : MonoBehaviour
{
    private MoveCenter moveCenter;
    private Projectile bullet;
    public int attackSpeed;

    private void Start()
    {
        moveCenter = GetComponentInParent<MoveCenter>();
        bullet = GetComponentInChildren<Projectile>();
    }

    public void SpawnProjectile()
    {
        transform.LookAt(moveCenter.destination);
        bullet.transform.position = transform.position;
        bullet.moving = true;
    }

    public void Attack()
    {
        InvokeRepeating("SpawnProjectile", attackSpeed, attackSpeed);
    }

    public void CeaseFire()
    {
        CancelInvoke("SpawnProjectile");
        bullet.moving = false;
    }
}
