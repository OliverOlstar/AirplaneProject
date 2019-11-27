using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysicsNew : MonoBehaviour
{
    private Rigidbody _rb;

    public Vector3 _velocity;
    public Vector3 _lift;
    public Vector3 _gravity;
    public Vector3 _drag;
    public float thrust;
    [SerializeField] private float _windSpeed = 1;
    [SerializeField] private float _coefficient = 1;
    [SerializeField] private float _airDensity = 1;
    [SerializeField] private float _referenceArea = 1;
    [SerializeField] private float _wingArea = 1;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _velocity += transform.forward * thrust * Time.deltaTime;

        _velocity += _gravity * Time.deltaTime;

        float liftMagnitude = _coefficient * _airDensity * Mathf.Pow(_velocity.magnitude, 2) / 2 * _wingArea;
        _lift = transform.up * liftMagnitude;
        _velocity += _lift * Time.deltaTime;

        float dragMagnitude = _coefficient * _airDensity * Mathf.Pow(_velocity.magnitude, 2) / 2 * _referenceArea;
        _drag = -_velocity.normalized * dragMagnitude;
        _velocity += _drag * Time.deltaTime;


        //transform.position += _velocity * Time.deltaTime;
    }
}
