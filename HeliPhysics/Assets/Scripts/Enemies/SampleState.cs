using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleState : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    private bool _enabled = false;

    public void Setup(Transform pTarget, PlaneController pController, PlanePhysics pPhysics)
    {
        _target = pTarget;
        _controller = pController;
        _physics = pPhysics;
    }

    public void Enter()
    {
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

    }
}
