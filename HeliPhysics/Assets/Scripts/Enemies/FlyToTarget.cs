using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToTarget : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    [SerializeField] private float pitchMult = 0.01f;
    [SerializeField] private int _subState = 0;

    private bool _enabled = false;

    public void Setup(Transform pTarget, PlaneController pController, PlanePhysics pPhysics)
    {
        _target = pTarget;
        _controller = pController;
        _physics = pPhysics;
    }

    public void Enter()
    {
        _controller.thrust = true;
        _enabled = true;
    }

    public void Exit()
    {
        _enabled = false;
    }

    public bool CanEnter(float pDistance)
    {
        return true;
    }

    public bool CanExit(float pDistance)
    {
        return true;
    }

    public void Tick()
    {
        _controller.pitch = _physics._verticalVelocity.y * pitchMult;
        _controller.roll = -_physics._angluarVelocity.y;
    }
}
