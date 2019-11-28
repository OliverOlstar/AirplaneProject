using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    private Rigidbody _rb;

    public Vector3 _horizontalVelocity;
    public Vector3 _verticalVelocity;
    public Vector3 _lift;
    public Vector3 _gravity;
    public Vector3 _horizontalDrag;
    public Vector3 _verticalDrag;
    public float thrust;
    [SerializeField] private float _windSpeed = 1;
    [SerializeField] private float _dragVFactor;
    [SerializeField] private float _dragVMult;
    [SerializeField] private float _dragVDefault;
    [SerializeField] private float _dragHFactor;
    [SerializeField] private float _dragHMult;
    [SerializeField] private float _dragHDefault;
    [SerializeField] private float _liftMax;
    [SerializeField] private float _mass;
    [SerializeField] [Range(0.5f, 1)] private float _bestLiftAngle;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _horizontalVelocity = transform.forward * thrust;
        _horizontalVelocity.y = 0;

        _verticalVelocity += _gravity * Time.deltaTime;

        CalculateLift();
        _verticalVelocity += _lift * Time.deltaTime;

        CalculateVerticalDrag();
        _verticalVelocity += _verticalDrag * Time.deltaTime;

        CalculateHorizontalDrag();
        _horizontalVelocity += _horizontalDrag * Time.deltaTime;

        //_drag = _velocity.normalized * _dragDefault + _velocity * _dragFactor;
        //_velocity -= _drag * Time.deltaTime;

        Vector3 _velocity = _verticalVelocity /*+ _horizontalVelocity*/;
        transform.position += _velocity * Time.deltaTime * 5;
    }

    private void CalculateLift()
    {
        float angle = transform.localEulerAngles.x;

        if (angle < 90)
            angle = 180 - angle;
        if (angle > 270)            
            angle = 360 - angle + 180;

        float angle01 = (angle - 90) / 180;
        float liftMag = GetQuadraticCurveValue(angle01) * _liftMax;

        //Debug.Log(transform.localEulerAngles.x + " | Angle01: " + angle01 + " | LiftMag: " + liftMag);
        _lift = Vector3.Project(transform.up * liftMag, Vector3.up);
    }

    public float GetQuadraticCurveValue(float t)
    {
        float racio = 1 / _bestLiftAngle;
        float inverseRacio = 1 / (1 - _bestLiftAngle);

        float p1 = 0; float p2 = 0; float p3 = 0;

        if (t <= _bestLiftAngle)
        {
            t *= racio;
            p1 = 0;
            p2 = -0.43f;
            p3 = 1;
        }
        else if (t > _bestLiftAngle)
        {
            t = ((t - 1) * inverseRacio) + 1;
            p1 = 1;
            p2 = 0.1f;
            p3 = 0;
        }

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return ((uu * p1) + (2 * u * t * p2) + (tt * p3)) * 2;
    }

    private void CalculateVerticalDrag()
    {
        float dragMagnitude = (Mathf.Pow(_verticalVelocity.y * _dragVFactor, 2) - _dragVDefault) / 2 * _dragVMult;
        float direction = _verticalVelocity.y / Mathf.Abs(_verticalVelocity.y);
        _verticalDrag = dragMagnitude * Vector3.up * -direction;
    }

    private void CalculateHorizontalDrag()
    {
        float dragMagnitude = (Mathf.Pow(_horizontalVelocity.y * _dragHFactor, 2) - _dragHDefault) / 2 * _dragHMult;

        Vector3 direction = _horizontalVelocity;
        direction.y = 0;
        direction.Normalize();

        _horizontalDrag = dragMagnitude *  -direction;
    }
}
