using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private Vector3 _velocity;
    [SerializeField] private Vector3 _lift;
    [SerializeField] private Vector3 _gravity;
    public float thrust;
    [SerializeField] private float _windSpeed = 1;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _velocity += transform.forward * thrust * Time.deltaTime;

        _velocity += _gravity * Time.deltaTime;

        _lift = transform.up * thrust * _windSpeed;
        _velocity += _lift * Time.deltaTime;


        transform.position += _velocity * Time.deltaTime;
    }
}
