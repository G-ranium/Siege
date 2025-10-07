using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private MoveCenter moveCenter;
    private EnemyAttacking enemyAttacking;

    public bool attacking;
    // Start is called before the first frame update
    void Start()
    {
        EnemyTracker.Instance.RegisterEnemy();
        moveCenter = GetComponent<MoveCenter>();
        enemyAttacking = GetComponentInChildren<EnemyAttacking>();
        attacking = false;
    }

    void OnDestroy()
    {
        if (EnemyTracker.Instance != null)
            EnemyTracker.Instance.UnregisterEnemy();
    }

    public void Update()
    {
        if (moveCenter != null && moveCenter.speed == 0 && !attacking)
        {
            Debug.Log("Load cannon");
            enemyAttacking.Attack();
            attacking = true;
        }

        if (moveCenter.speed != 0 && attacking)
        {
            enemyAttacking.CeaseFire();
            attacking = false;
        }
    }
}
