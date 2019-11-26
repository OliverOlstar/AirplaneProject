using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private float yawnSpeed = 5;
    [SerializeField] private float pitchSpeed = 5;
    [SerializeField] private float rollSpeed = 5;

    [Space]
    [SerializeField] private float TurnDampening = 10;
    private Vector3 _LocalRotation;

    void Start()
    {
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

        //Rutter
        transform.Rotate(Vector3.up, Time.deltaTime * yawnSpeed * Input.GetAxis("Yawn"));

        //Elevators
        transform.Rotate(Vector3.right, Time.deltaTime * pitchSpeed * Input.GetAxis("Pitch"));

        //Ailerons
        transform.Rotate(Vector3.forward, Time.deltaTime * rollSpeed * -Input.GetAxis("Roll"));
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
