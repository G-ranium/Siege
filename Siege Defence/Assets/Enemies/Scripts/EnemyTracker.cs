using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public static EnemyTracker Instance;
    public IntData activeEnemies;

    //public int activeEnemies = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy()
    {
        activeEnemies.UpdateValue(1);
    }

    public void UnregisterEnemy()
    {
        activeEnemies.UpdateValue(-1);
        if ((int)activeEnemies.Value < 0) activeEnemies.Value = 0;
    }
}
