using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCenter : MonoBehaviour
{
    public float speed;
    public float range;
    private float stopSpeed;
    //public bool attacking;
    private Vector3 centerPoint;
    public Vector3 destination;

    private Transform targetObject = null;

    void OnTriggerEnter(Collider other)
    {
        targetObject = other.transform;
        stopSpeed = speed;
    }

    void OnTriggerExit(Collider other)
    {
        targetObject = null;
        destination = centerPoint;
        speed = stopSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyAttacking attackBehavior = GetComponent<EnemyAttacking>();
        centerPoint = new Vector3(0f, 1f, 0f);
        destination = centerPoint;

        transform.LookAt(centerPoint);
    }

    // Update is called once per frame
    void Update()
    {

        if (targetObject != null)
        {
            destination = targetObject.position;
        }

        if (targetObject == null)
        {
            destination = centerPoint;
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (targetObject != null && Vector3.Distance(transform.position, destination) <= range)
        {
            speed = 0;
        }

        transform.LookAt(destination);

        if (Vector3.Distance(transform.position, centerPoint) < 0.1f && targetObject == null)
        {
            Destroy(gameObject);
        }
    }
}
