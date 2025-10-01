using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int level;
    public float health;
    public float fireRate;
    public float damage;
    public float range;
}