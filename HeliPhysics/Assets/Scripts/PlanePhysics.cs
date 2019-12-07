using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanePhysics : MonoBehaviour
{
    private Rigidbody _rb;
    private PlaneWheels[] _wheels;

    public Vector3 _velocity;
    public Vector3 _horizontalVelocity;
    public Vector3 _verticalVelocity;
    public Vector3 _lift;
    public Vector3 _gravity;
    public Vector3 _horizontalDrag;
    public Vector3 _verticalDrag;
    public Vector3 _angluarVelocity;
    public float _verticalEffectOnHorizontal;
    public float thrust;
    //[SerializeField] private Vector3 _wind;
    [SerializeField] private float _dragVFactor;
    [SerializeField] private float _dragVMult;
    [SerializeField] private float _dragVDefault;
    [SerializeField] private float _dragHFactor;
    [SerializeField] private float _dragHMult;
    [SerializeField] private float _dragHDefault;
    [SerializeField] private float _liftMultH;
    [SerializeField] private float _liftMinH = 0.1f;
    [SerializeField] private float _liftMax;
    [SerializeField] private float _VoHMultDown;
    [SerializeField] private float _VoHMultUp;

    [SerializeField] private float _AngluarInfluence;
    //[SerializeField] private float _mass;
    [SerializeField] [Range(0.5f, 1)] private float _bestLiftAngle;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _wheels = GetComponentsInChildren<PlaneWheels>();
    }

    void Update()
    {
        if (Time.deltaTime >= 0.5f) return;

        _horizontalVelocity += transform.forward * thrust * Time.deltaTime;
        _horizontalVelocity.y = 0;

        CalculateHorizontalDrag();
        _horizontalVelocity += _horizontalDrag * Time.deltaTime;

        _verticalVelocity += _gravity * Time.deltaTime;

        CalculateLift();
        _verticalVelocity += _lift * Time.deltaTime;

        CalculateVerticalDrag();
        _verticalVelocity += _verticalDrag * Time.deltaTime;

        CalculateVerticalEffectOnHorizontal();
        _horizontalVelocity += _verticalEffectOnHorizontal * _horizontalVelocity.normalized * Time.deltaTime;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        _horizontalVelocity = _horizontalVelocity.magnitude * forward;

        //Calculate Angular;
        _angluarVelocity = Vector3.Project(-transform.right, Vector3.up) * Time.deltaTime * _AngluarInfluence * _horizontalVelocity.magnitude;

        _velocity = _verticalVelocity + _horizontalVelocity;
        //TODO Check with wheels

        CalculateWheelsForces();
        _verticalVelocity.y = _velocity.y;
        _horizontalVelocity = new Vector3(_velocity.x, 0, _velocity.z);


        transform.position += _velocity * Time.deltaTime * 5;
        transform.localEulerAngles += _angluarVelocity;
    }

    private void CalculateWheelsForces()
    {
        foreach (PlaneWheels wheel in _wheels)
        {
            wheel.CheckWithWheel();
        }
    }

    private void CalculateLift()
    {
        float angle = transform.localEulerAngles.x;

        if (angle < 90)
            angle = 180 - angle;
        if (angle > 270)            
            angle = 360 - angle + 180;

        float angle01 = (angle - 90) / 180;
        float quadraticValue = GetQuadraticCurveValue(angle01, _bestLiftAngle, -0.5f, -0.8f, 1, 1, 0.1f, 0);
        float liftMag = quadraticValue * _liftMax * (_horizontalVelocity.magnitude * _liftMultH + _liftMinH);

        //Debug.Log(transform.localEulerAngles.x + " | Angle01: " + angle01 + " | LiftMag: " + liftMag);
        _lift = Mathf.Abs(transform.up.y) * liftMag * Vector3.up;
    }

    public float GetQuadraticCurveValue(float t, float pMid, float p1_1, float p1_2, float p1_3, float p2_1, float p2_2, float p2_3)
    {
        float racio = 1 / _bestLiftAngle;
        float inverseRacio = 1 / (1 - _bestLiftAngle);

        float p1 = 0; float p2 = 0; float p3 = 0;

        if (t <= _bestLiftAngle)
        {
            t *= racio;
            p1 = p1_1;
            p2 = p1_2;
            p3 = p1_3;
        }
        else if (t > pMid)
        {
            t = ((t - 1) * inverseRacio) + 1;
            p1 = p2_1;
            p2 = p2_2;
            p3 = p2_3;
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
        float dragMagnitude = (Mathf.Pow(_horizontalVelocity.magnitude * _dragHFactor, 2) - _dragHDefault) / 2 * _dragHMult;

        Vector3 direction = _horizontalVelocity;
        direction.y = 0;
        direction.Normalize();

        _horizontalDrag = dragMagnitude * -direction;
    }

    private void CalculateVerticalEffectOnHorizontal()
    {
        float mult = (transform.forward.y < 0 ? _VoHMultDown : _VoHMultUp);
        float angleMult = GetQuadraticCurveValue(transform.forward.y, 0.5f, 0, 0.5f, 1, 1, 1.5f, 0);

        // TODO Make a thresh hold or porabla rather than instant
        if (transform.forward.y < 0.7f && transform.forward.y > -0.85f)
        {
            //Add Horizontal based on vertical velocity
            _verticalEffectOnHorizontal = angleMult * _verticalVelocity.y * mult;
        }
        else
        {
            //Reduce Horizontal if plane is facing up
            _verticalEffectOnHorizontal = -_horizontalVelocity.magnitude * 0.999f;
        }

    }
}
