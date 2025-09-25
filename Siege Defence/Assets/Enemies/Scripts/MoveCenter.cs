using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCenter : MonoBehaviour
{
    public float speed;
    public float range;
    private float stopSpeed;
    private Vector3 centerPoint;
    private Vector3 destination;

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
        centerPoint = new Vector3(0f, 1f, 0f);
        destination = centerPoint;

        transform.LookAt(centerPoint);
        //transform.Rotate(90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 destination = centerPoint;

        if (targetObject != null)
        {
            destination = targetObject.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (targetObject != null && Vector3.Distance(transform.position, destination) <= range)
            speed = 0;

        transform.LookAt(destination);

        if (Vector3.Distance(transform.position, centerPoint) < 0.1f && targetObject == null)
        {
            Destroy(gameObject);
        }
    }
}
