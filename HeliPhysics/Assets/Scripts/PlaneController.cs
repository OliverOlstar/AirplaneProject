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
        //Quaternion rotation = transform.rotation;

        if (Input.GetButtonDown("Jump"))
            physics.thrust = maxThrust;

        if (Input.GetButtonUp("Jump"))
            physics.thrust = minThrust;


        //Rutter
        transform.Rotate(Vector3.up, Time.deltaTime * yawnSpeed * Input.GetAxis("Yawn") * Mathf.Min(physics._horizontalVelocity.magnitude, 1));

        //Elevators
        if (physics._horizontalVelocity.magnitude > speedNeededToPitch / 5)
            transform.Rotate(Vector3.right, Time.deltaTime * pitchSpeed * Input.GetAxis("Pitch") * Mathf.Min(physics._horizontalVelocity.magnitude / 5 - speedNeededToPitch / 5, 1));

        //Ailerons
        if (physics._horizontalVelocity.magnitude > speedNeededToRoll / 5)
            transform.Rotate(Vector3.forward, Time.deltaTime * rollSpeed * -Input.GetAxis("Roll") * Mathf.Min(physics._horizontalVelocity.magnitude / 5 - speedNeededToRoll / 5, 1));
    }

    private void LerpRotation()
    {
        _LocalRotation += Time.deltaTime * yawnSpeed * Input.GetAxis("Pitch") * Vector3.right;
        _LocalRotation += Time.deltaTime * rollSpeed * -Input.GetAxis("Roll") * Vector3.forward;

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
}
