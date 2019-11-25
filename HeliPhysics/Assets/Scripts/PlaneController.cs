using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [SerializeField] private float yawnSpeed = 5;
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
        Rotation();
    }

    private void Rotation()
    {
        transform.Rotate(Vector3.right, Time.deltaTime * yawnSpeed * Input.GetAxis("Vertical"));
        transform.Rotate(Vector3.forward, Time.deltaTime * rollSpeed * -Input.GetAxis("Horizontal"));
    }

    private void LerpRotation()
    {
        _LocalRotation += Time.deltaTime * yawnSpeed * Input.GetAxis("Vertical") * Vector3.right;
        _LocalRotation += Time.deltaTime * rollSpeed * -Input.GetAxis("Horizontal") * Vector3.forward;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - transform.right, transform.position + transform.right);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position - transform.forward, transform.position + transform.forward);
    }
}
