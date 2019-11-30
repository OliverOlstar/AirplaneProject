using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneVisuals : MonoBehaviour
{
    public Transform _target;
    [SerializeField] private float rotationDampening = 5;
    [SerializeField] private float positionDampening = 5;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _target.position;
        transform.rotation = _target.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_target == null) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, Time.deltaTime * rotationDampening);
        //transform.position = Vector3.Lerp(transform.position, _target.position, Time.deltaTime * positionDampening);
        transform.position = _target.position;
    }
}
