using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private PlanePhysics physics;

    [SerializeField] private float yawnSpeed = 5;
    [SerializeField] private float pitchSpeed = 5;
    [SerializeField] private float rollSpeed = 5;

    [Space]
    [SerializeField] private float TurnDampening = 10;
    private Vector3 _LocalRotation;

    [Space]
    [SerializeField] private float speedNeededToPitch = 2;
    [SerializeField] private float speedNeededToRoll = 2;

    [Space]
    [SerializeField] private float maxThrust = 10;
    [SerializeField] private float minThrust = 0;
    [SerializeField] private float thrustRampupSpeed = 1;

    [Header("Inputs")]
    public float pitch;
    public float yawn;
    public float roll;
    public bool thrust;
    private bool thrusting = false;

    void Start()
    {
        physics = GetComponent<PlanePhysics>();
        _LocalRotation = transform.localEulerAngles;
    }

    void Update()
    {
        //Change Effect based on current speed
        Rotation();
    }

    private void Rotation()
    {
        //Thrust
        if (thrust != thrusting)
        {
            StopCoroutine("rampupThrust");
            StartCoroutine("rampupThrust", (thrust ? 1 : -1));
            thrusting = thrust;
        }

        //Rutter - Yawn
        transform.Rotate(Vector3.up, Time.deltaTime * yawnSpeed * yawn * Mathf.Min(physics._horizontalVelocity.magnitude, 1));

        //Elevators - Pitch
        if (physics._horizontalVelocity.magnitude > speedNeededToPitch)
            transform.Rotate(Vector3.right, Time.deltaTime * pitchSpeed * pitch * Mathf.Min(physics._horizontalVelocity.magnitude / 5 - speedNeededToPitch / 5, 1));

        //Ailerons - Roll
        if (physics._horizontalVelocity.magnitude > speedNeededToRoll)
            transform.Rotate(Vector3.forward, Time.deltaTime * rollSpeed * -roll * Mathf.Min(physics._horizontalVelocity.magnitude / 5 - speedNeededToRoll / 5, 1));
    }

    private void LerpRotation()
    {
        _LocalRotation += Time.deltaTime * yawnSpeed * pitch * Vector3.right;
        _LocalRotation += Time.deltaTime * rollSpeed * -roll * Vector3.forward;

        Quaternion TargetQ = Quaternion.Euler(_LocalRotation);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, TargetQ, Time.deltaTime * TurnDampening);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    IEnumerator rampupThrust(int pDirection)
    {
        while (pDirection == 1 ? physics.thrust <= maxThrust : physics.thrust >= minThrust)
        {
            yield return null;
            physics.thrust += thrustRampupSpeed * Time.deltaTime * pDirection * (physics._horizontalVelocity.magnitude + 4f) / 40;
        }

        physics.thrust = pDirection == 1 ? maxThrust : minThrust;
    }
}
