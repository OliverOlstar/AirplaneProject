using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneVisuals : MonoBehaviour
{
    public Transform _target;
    private PlanePhysics _physics;
    private PlaneController _controller;
    [SerializeField] private float rotationDampening = 5;
    [SerializeField] private CameraPivot camera;

    [Space]
    [SerializeField] private Transform propeller;
    [SerializeField] private float propellerSpeed;

    void Start()
    {
        transform.position = _target.position;
        transform.rotation = _target.rotation;
        _physics = transform.parent.GetChild(0).GetComponent<PlanePhysics>();
        _controller = transform.parent.GetChild(0).GetComponent<PlaneController>();
    }

    void LateUpdate()
    {
        if (_target == null) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, Time.deltaTime * rotationDampening);
        transform.position = _target.position;

        if (camera != null) camera.tick(transform.position);

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        propeller.Rotate(Vector3.forward * _physics.thrust * Time.deltaTime * propellerSpeed);
    }
}
