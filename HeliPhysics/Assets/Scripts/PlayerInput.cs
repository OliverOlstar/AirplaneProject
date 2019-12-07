using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlaneController _plane;

    // Start is called before the first frame update
    void Start()
    {
        _plane = GetComponentInChildren<PlaneController>();
    }

    // Update is called once per frame
    void Update()
    {
        _plane.thrust = Input.GetButton("Jump");
        _plane.yawn = Input.GetAxis("Yawn");
        _plane.pitch = Input.GetAxis("Pitch");
        _plane.roll = Input.GetAxis("Roll");
    }
}
