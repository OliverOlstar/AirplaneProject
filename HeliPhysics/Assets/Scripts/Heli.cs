using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heli : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 gravity;
    private Vector3 upDirection;
    [SerializeField] private float upForce = 5;
    [SerializeField] private float upForceInput = 5;

    [Space]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private float upAmount;

    [Space]
    [SerializeField] private float gizmoDisplayLength = 1;

    private Transform _camera;

    [Header("Roter")]
    [SerializeField] private Transform roter;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float inputSpinSpeed;

    private void Start()
    {
        _camera = Camera.main.transform.parent;
    }

    void Update()
    {
        velocity += gravity * Time.deltaTime;

        inputVector = new Vector3(Input.GetAxis("Horizontal"),0 , Input.GetAxis("Vertical"));
        inputVector = _camera.TransformDirection(inputVector);
        inputVector.y = 0;
        inputVector.Normalize();
        upDirection = new Vector3(inputVector.x, upAmount, inputVector.z).normalized;

        if (Input.GetButton("Jump"))
        {
            velocity += upDirection * upForceInput * Time.deltaTime;
            roter.Rotate(Vector3.up * Time.deltaTime * inputSpinSpeed);
        }
        else
        {
            velocity += upDirection * upForce * Time.deltaTime;
            roter.Rotate(Vector3.up * Time.deltaTime * spinSpeed);
        }

        //Quaternion targetRot = Quaternion.LookRotation(upDirection);
        //targetRot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 5);
        //transform.rotation = targetRot;

        //transform.rotation = Quaternion.LookRotation();

        GetComponent<Rigidbody>().position += velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + upDirection * (Input.GetButton("Jump") ? upForceInput : upForce) * gizmoDisplayLength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + gravity * gizmoDisplayLength);
    }
}
