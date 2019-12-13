using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOff : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    [SerializeField] private LandingStrip pointA;

    [Space]
    [SerializeField] private float pullUpSpeed;
    [SerializeField] private float pullUpLength = 1.5f;
    [SerializeField] [Range(0, -1)] private float pitchUpSpeed = -0.5f;
    [SerializeField] [Range(0, 1)] private float pullUpTargetPitch = 0.2f;
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

                float pointARotY = pointA.transform.eulerAngles.y % 180;
                float myRotY = transform.GetChild(0).eulerAngles.y % 180;

                //Rotate to be parallel to the landing strip
                if (myRotY < pointARotY)
                {
                    Debug.Log("myRotY: " + myRotY + " | pointARotY: " + pointARotY);
                    _controller.yawn = -Vector3.Dot(transform.GetChild(0).forward, pointA.transform.forward);
                } 
                else if (myRotY > pointARotY)
                {
                    Debug.Log("myRotY: " + myRotY + " | pointARotY: " + pointARotY);
                    _controller.yawn = Vector3.Dot(transform.GetChild(0).forward, pointA.transform.forward);
                }

                //If outside of runway turn onto runway
                Vector3 directionBetween = transform.GetChild(0).position - pointA.transform.position;
                float disFromPointA = Vector3.Project(directionBetween, pointA.transform.right).magnitude - (pointA.stripWidth / 2);
                disFromPointA = Mathf.Min(disFromPointA, 5);

                if (disFromPointA > -2)
                {
                    float dotRight = Vector3.Dot(directionBetween.normalized, pointA.transform.right);
                    float dotLeft = Vector3.Dot(directionBetween.normalized, -pointA.transform.right);
                    Debug.Log("Rot Right: " + dotRight + " | Rot Left: " + dotLeft);

                    if (dotRight > dotLeft)
                    {
                        if (dotRight > 0.1f)
                            _controller.yawn = disFromPointA / 4;
                    }
                    else
                    {
                        if (dotLeft > 0.1f)
                            _controller.yawn = -disFromPointA / 4;
                    }
                }


                if (_physics._horizontalVelocity.magnitude >= pullUpSpeed)
                {
                    _subState = 1;
                    _controller.yawn = 0;
                }

                break;

            case 1:
                //Get off the ground
                _controller.pitch = pitchUpSpeed;

                if (transform.GetChild(0).forward.y >= pullUpTargetPitch)
                {
                    leaveStateTime = Time.time + pullUpLength;
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
