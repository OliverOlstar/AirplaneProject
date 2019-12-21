using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVbySpeed : MonoBehaviour
{
    [HideInInspector] public PlanePhysics physics;
    [SerializeField] private float minFOV;
    [SerializeField] private float maxFOV;
    [SerializeField] private float velMult;
    [SerializeField] private float velSubtract;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (physics != null)
        _camera.fieldOfView = Mathf.Clamp((physics._velocity.magnitude - velSubtract) * velMult + minFOV, minFOV, maxFOV);
    }
}
