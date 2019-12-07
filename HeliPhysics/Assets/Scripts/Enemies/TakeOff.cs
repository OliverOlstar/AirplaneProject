﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOff : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    [SerializeField] private float pullUpSpeed;
    [SerializeField] private int _subState = 0;
    private float leaveStateTime = 0;

    private bool _enabled = false;

    public void Setup(Transform pTarget, PlaneController pController, PlanePhysics pPhysics)
    {
        _target = pTarget;
        _controller = pController;
        _physics = pPhysics;
    }

    public void Enter()
    {
        _subState = 0;
        _controller.thrust = true;
        _enabled = true;
    }

    public void Exit()
    {
        _enabled = false;
    }

    public bool CanEnter(float pDistance)
    {
        return _subState != 3;
    }

    public bool CanExit(float pDistance)
    {
        return _subState == 3;
    }

    public void Tick()
    {
        switch(_subState)
        {
            case 0:
                //Acceleration
                if (_physics._horizontalVelocity.magnitude >= pullUpSpeed)
                {
                    _subState = 1;
                }

                break;

            case 1:
                //Get off the ground
                _controller.pitch = -0.5f;

                Debug.Log(transform.GetChild(0).forward.y);
                if (transform.GetChild(0).forward.y >= 0.2f)
                {
                    leaveStateTime = Time.time + 1;
                    _controller.pitch = 0f;
                    _subState = 2;
                }

                break;

            case 2:
                if (Time.time >= leaveStateTime)
                    _subState = 3;

                break;
        }
    }
}
