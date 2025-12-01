using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int speed = 10;
    public int damage = 10;
    public bool moving;
    public GameObject owner; // Set this to the shooter (tower or enemy)
    private MeshRenderer meshRenderer;

    private void Start()
    {
        moving = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    void Update()
    {
        if (moving == true)
        {
            meshRenderer.enabled = true;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            transform.position = owner.transform.position;
            moving = false;
        }
        else
        {
            transform.position = owner.transform.position;
            moving = false;
        }
    }

    public void ToggleVisibility()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = !meshRenderer.enabled;
        }
    }
}