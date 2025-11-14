using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int level;
    public float health;
    public float maxHealth;
    public float fireRate;
    public int damage;
    public float range;
    
    [Header("Targeting")]
    public LayerMask enemyLayer;
}