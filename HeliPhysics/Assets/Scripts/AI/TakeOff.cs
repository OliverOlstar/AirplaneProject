using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOff : MonoBehaviour, IState
{
    private PlaneController _controller;
    private PlanePhysics _physics;

    public LandingStrip pointA;

    [SerializeField] private int _subState = 0;
    [SerializeField] private bool _enabled = false;
    private float leaveStateTime = 0;

    [Header("Pitch")]
    [SerializeField] private float pullUpSpeed;
    [SerializeField] private float pullUpLength = 1.5f;
    [SerializeField] [Range(0, -1)] private float pitchUpSpeed = -0.5f;
    [SerializeField] [Range(0, 1)] private float pullUpTargetPitch = 0.2f;

    [Header("Yawn")]
    [SerializeField] private float parallelRotMult = 1;
    [SerializeField] private float returnToStripRotMult = 1;
    [SerializeField] private float returnToStripAngle = 20;
    [SerializeField] private float returnToStripBuffer = 1f;
    [SerializeField] private float returnToStripMaxThrust = 3;


    public void Setup(PlaneController pController, PlanePhysics pPhysics)
    {
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

    public bool CanExit()
    {
        return _subState == 3;
    }

    public void Tick()
    {
        switch (_subState)
        {
            case 0:
                //Acceleration
                StayOnLandingStrip();

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

    private void StayOnLandingStrip()
    {
        float relRotY = Vector3.SignedAngle(transform.GetChild(0).forward, pointA.transform.forward, Vector3.up);

        //If outside of runway turn onto runway
        Vector3 directionBetween = transform.GetChild(0).position - pointA.transform.position;
        float disFromPointA = Vector3.Project(directionBetween, pointA.transform.right).magnitude - (pointA.stripWidth / 2);
        disFromPointA = Mathf.Min(disFromPointA, 10);

        float dotRight = Vector3.Dot(directionBetween.normalized, pointA.transform.right);

        float targetAngleMax = returnToStripAngle + returnToStripBuffer;
        float targetAngleMin = returnToStripAngle - returnToStripBuffer;

        //Am I outside the strip
        if (disFromPointA > -2)
        {
            float left = dotRight > 0 ? 1 : -1;
            relRotY *= left;

            // Am I within the target return angle
            if (relRotY < targetAngleMax && relRotY > targetAngleMin)
            {
                _controller.yawn = 0;
            }
            else
            {
                // Rotate to face target return angle
                if (relRotY > returnToStripAngle)
                    _controller.yawn = disFromPointA / 8 * returnToStripRotMult * left;
                else
                    _controller.yawn = disFromPointA / 8 * returnToStripRotMult * -left;
            }

            //Slow Down if outside strip
            if (_physics.thrust >= returnToStripMaxThrust)
            {
                _controller.thrust = false;
            }
            else
            {
                _controller.thrust = true;
            }
        }
        else
        {
            // Become Parallel and Speed up
            if (relRotY != 0)
            {
                _controller.yawn = Mathf.Clamp(relRotY / 15, -1, 1) * parallelRotMult;
            }

            _controller.thrust = true;
        }
    }
}
