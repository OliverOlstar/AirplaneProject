using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody _rb;

    private Vector3 velocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        velocity = transform.forward * Time.deltaTime * speed;

        transform.position += velocity * Time.deltaTime;
    }
}
