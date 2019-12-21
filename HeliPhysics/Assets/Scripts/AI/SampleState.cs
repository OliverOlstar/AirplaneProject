using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleState : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    private bool _enabled = false;

    public void Setup(PlaneController pController, PlanePhysics pPhysics)
    {
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

    public bool CanExit()
    {
        return true;
    }

    public void Tick()
    {

    }
}
