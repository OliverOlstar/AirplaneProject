using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToTarget : MonoBehaviour, IState
{
    private Transform _target;
    private PlaneController _controller;
    private PlanePhysics _physics;

    [SerializeField] private LandingStrip pointB;
    [SerializeField] private int _subState = 0;

    [SerializeField] private bool _enabled = false;
    private bool _canExit = false;

    [Space]
    [SerializeField] private float pitchMult = 0.01f;
    [SerializeField] private float yawnSpeed;
    [SerializeField] private float rollSpeed = 1;
    [SerializeField] private float rollSpeedParallel = 1;

    [SerializeField] private float targetPitch = 0;
    [SerializeField] [Range(-0.5f, 0.5f)] private float targetRoll = 0;
    [SerializeField] [Range(0, 0.5f)] private float preParallelingRoll = 0.1f;
    [SerializeField] private float targetRelAngle = 16;
    [SerializeField] private float targetAngleBuffer = 1;
    [SerializeField] private float rollClamp = 0.25f;

    [SerializeField] private float stripBuffer = 2;

    [Space]
    [SerializeField] private float decendStartDistance = 1000;
    [SerializeField] private float leaveStateDistance = 500;

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
        _canExit = false;
    }

    public void Exit()
    {
        _enabled = false;
    }

    public bool CanExit()
    {
        return _canExit;
    }

    public void Tick()
    {
        float distance = Vector3.Distance(transform.GetChild(0).position, pointB.transform.position);

        if (distance <= leaveStateDistance)
        {
            _canExit = true;
        }
        else if (distance <= decendStartDistance)
        {
            _controller.thrust = false;
            targetPitch = -1f;
        }

        LevelOut();
        StayOnLandingStrip();
    }

    private void LevelOut()
    {
        _controller.pitch = (_physics._verticalVelocity.y - targetPitch) * pitchMult;
        _controller.roll = -_physics._angluarVelocity.y + targetRoll;
        _controller.yawn = 0;
    }

    private void StayOnLandingStrip()
    {
        // Rotate to be parallel to the landing strip
        float relRotY = transform.GetChild(0).eulerAngles.y - pointB.transform.eulerAngles.y + 180;

        // If outside of runway turn onto runway
        Vector3 directionBetween = transform.GetChild(0).position - pointB.transform.position;
        float disFromPointA = Vector3.Project(directionBetween, pointB.transform.right).magnitude - (pointB.stripWidth / 2);
        disFromPointA = Mathf.Min(disFromPointA, 500);

        if (disFromPointA > -stripBuffer)
        {
            float dotRight = Vector3.Dot(directionBetween.normalized, pointB.transform.right);
            float dotLeft = Vector3.Dot(directionBetween.normalized, -pointB.transform.right);

            float targetAngleMax = targetRelAngle + targetAngleBuffer;
            float targetAngleMin = targetRelAngle - targetAngleBuffer;

            Debug.Log(relRotY);
            if (dotRight > dotLeft)
            {
                Debug.Log("Right - Max: " + (relRotY < targetAngleMax) + " | Min: " + (relRotY > targetAngleMin));

                if (relRotY < targetAngleMax && relRotY > targetAngleMin)
                {
                    Debug.Log("In range");

                    targetRoll = 0;
                }
                else
                {
                    Debug.Log("Out of range");

                    if (relRotY > targetRelAngle)
                        targetRoll = -preParallelingRoll;
                    else
                        targetRoll = preParallelingRoll;
                }
            }
            else
            {
                Debug.Log("Left");

                if (relRotY > -targetAngleMax && relRotY < -targetAngleMin)
                {
                    Debug.Log("In range");

                    targetRoll = 0;
                }
                else
                {
                    Debug.Log("Out of range");

                    if (relRotY > -targetRelAngle)
                        targetRoll = -preParallelingRoll;
                    else
                        targetRoll = preParallelingRoll;
                }
            }
        }
        else
        {
            Debug.Log("PARALLEL");

            // Become Parallel
            if (relRotY < 0)
            {
                targetRoll = Mathf.Pow(Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward), 2) * rollSpeedParallel;
            }
            else if (relRotY != 0)
            {
                targetRoll = -Mathf.Pow(Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward), 2) * rollSpeedParallel;
            }

            // Become Parallel
            //if (relRotY != 0)
            //{
            //    _controller.roll = Mathf.Clamp(relRotY / 180, -1, 1) * rollSpeedParallel;
            //}
        }

        targetRoll = Mathf.Clamp(targetRoll, -rollClamp, rollClamp);
    }
}
