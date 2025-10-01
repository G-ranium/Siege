using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int speed = 10;
    public int damage = 10;
    public GameObject owner; // Set this to the shooter (tower or enemy)

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with the owner
        if (other.gameObject == owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject); // Optional: destroy on any other collision
        }
    }
}