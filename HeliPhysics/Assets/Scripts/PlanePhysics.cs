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
    public Vector3 _drag;
    public Vector3 _verticalDrag;
    public float thrust;
    [SerializeField] private float _windSpeed = 1;
    [SerializeField] private float _dragFactor;
    [SerializeField] private float _dragDefault;
    [SerializeField] private float _liftMax;
    [SerializeField] [Range(0.5f, 1)] private float _bestLiftAngle;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //_velocity = Vector3.zero;

        _horizontalVelocity = transform.forward * thrust;
        _horizontalVelocity.y = 0;

        _verticalVelocity += _gravity * Time.deltaTime;

        CalculateLift();
        _verticalVelocity += _lift * Time.deltaTime;

        CalculateVerticalDrag();
        _verticalVelocity += _verticalDrag * Time.deltaTime;

        //_lift = transform.up * Vector3.Project(_velocity, transform.forward).magnitude * _windSpeed;
        //_lift = transform.up * thrust * _windSpeed;
        //_lift = Vector3.Project(_lift, Vector3.up);

        //_drag = _velocity.normalized * _dragDefault + _velocity * _dragFactor;
        //_velocity -= _drag * Time.deltaTime;

        Vector3 _velocity = _verticalVelocity + _horizontalVelocity;
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
            p2 = 0f;
            p3 = 1;
        }
        else if (t > _bestLiftAngle)
        {
            t = ((t - 1) * inverseRacio) + 1;
            p1 = 1;
            p2 = 0f;
            p3 = 0;
        }

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return ((uu * p1) + (2 * u * t * p2) + (tt * p3)) * 2;
    }

    private void CalculateVerticalDrag()
    {


        _verticalDrag = Vector3.one;
    }
}
