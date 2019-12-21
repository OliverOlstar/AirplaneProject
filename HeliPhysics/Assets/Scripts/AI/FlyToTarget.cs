using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToTarget : MonoBehaviour, IState
{
    private PlaneController _controller;
    private PlanePhysics _physics;

    public LandingStrip pointB;

    [SerializeField] private bool _enabled = false;
    private bool _canExit = false;

    [Space]
    [SerializeField] private float pitchMult = 0.01f;
    [SerializeField] private float yawnSpeed;
    //[SerializeField] private float rollSpeed = 1;
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

    [Space]
    [SerializeField] private float ODDistance = 10;
    [SerializeField] private float ODHorizontalVelMult = 2;

    [Header("Pitch")]

    [Header("Pitch To Target")]
    [SerializeField] private float RelHeightTargetHeight = 60;
    [SerializeField] private float relHeightMult = 0.1f;
    [SerializeField] private float relHeightPitchClamp = 3f;

    public void Setup(PlaneController pController, PlanePhysics pPhysics)
    {
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
            // Switch to Landing
            _canExit = true;
        }
        else if (distance <= decendStartDistance)
        {
            // Start Decending
            _controller.thrust = false;
            targetPitch = -1f;
        }

        LevelOut();

        StayOnLandingStrip();
        PitchUpToTarget();
        AvoidObsticals();
    }

    private void PitchUpToTarget()
    {
        float relHeight = (transform.GetChild(0).position.y - pointB.transform.position.y - RelHeightTargetHeight) * relHeightMult;
        targetPitch = Mathf.Clamp(-relHeight, -relHeightPitchClamp, relHeightPitchClamp);
    }

    private void LevelOut()
    {
        _controller.pitch = (_physics._verticalVelocity.y - targetPitch) * pitchMult;
        _controller.roll = -_physics._angluarVelocity.y + targetRoll;
        _controller.yawn = 0;
    }

    private void StayOnLandingStrip()
    {
        Vector3 directionBetween = transform.GetChild(0).position - pointB.transform.position;
        float disFromPointB = Vector3.Project(directionBetween, pointB.transform.right).magnitude - (pointB.stripWidth / 2);
        disFromPointB = Mathf.Min(disFromPointB, 500);

        float dotRight = Vector3.Dot(directionBetween.normalized, pointB.transform.right);
        float relRotY = Vector3.SignedAngle(transform.GetChild(0).forward, -pointB.transform.forward, Vector3.up);

        // If outside of runway turn onto runway
        if (disFromPointB > -stripBuffer)
        {
            float targetRelAngleTemp = Mathf.Min(targetRelAngle + Mathf.Pow(disFromPointB / 100, 2) * 2, 90);
            //Debug.Log("dis " + Mathf.Pow(disFromPointB / 100, 2) * 2);

            float targetAngleMax = targetRelAngleTemp + targetAngleBuffer;
            float targetAngleMin = targetRelAngleTemp - targetAngleBuffer;

            float Left = dotRight > 0 ? -1 : 1;
            relRotY *= Left;

            if (relRotY < targetAngleMax && relRotY > targetAngleMin)
            {
                // In Range
                targetRoll = 0;
            }
            else
            {
                // Out of Range
                if (relRotY > targetRelAngleTemp)
                    targetRoll = preParallelingRoll * Left;
                else
                    targetRoll = preParallelingRoll * -Left;
            }
        }
        else
        {
            // Become Parallel
            if (disFromPointB < -8)
            {
                targetRoll = -Mathf.Pow(relRotY / 2, 2) * rollSpeedParallel;
            }
            else
            {
                if (relRotY != 0)
                {
                    _controller.yawn = relRotY * yawnSpeed;
                }

                targetRoll = 0;
            }
        }

        // Prevent Over rolling
        targetRoll = Mathf.Clamp(targetRoll, -rollClamp, rollClamp);
    }

    private void AvoidObsticals()
    {
        bool leftHit = false, rightHit = false;
        Vector3 forward = _physics._velocity.normalized;
        float raycastDistance = ODDistance + _physics._horizontalVelocity.magnitude * ODHorizontalVelMult;
        RaycastHit hit;

        if (Physics.Raycast(transform.GetChild(0).position + transform.GetChild(0).right * 2.7f, forward, out hit, raycastDistance))
        {
            rightHit = true;
        }
        if (Physics.Raycast(transform.GetChild(0).position - transform.GetChild(0).right * 2.7f, forward, out hit, raycastDistance))
        {
            leftHit = true;
        }

        if (rightHit && !leftHit)
        {
            // Go Soft left
            targetRoll = -0.5f;
        }
        else if (leftHit && !rightHit)
        {
            // Go Soft right
            targetRoll = 0.5f;
        }
        else if (rightHit || leftHit)
        {
            // All hit check if it can do a soft left turn
            Vector3 rayDirection = (forward + transform.GetChild(0).right * 0.75f).normalized;
            if (!Physics.Raycast(transform.GetChild(0).position, rayDirection, out hit, raycastDistance))
            {
                // Roll Right
                targetRoll = 0.9f;
                return;
            }

            rayDirection = (forward - transform.GetChild(0).right * 0.75f).normalized;
            if (!Physics.Raycast(transform.GetChild(0).position, rayDirection, out hit, raycastDistance))
            {
                // Roll Left
                targetRoll = -0.9f;
                return;
            }

            // Can I just pitch up over it
            rayDirection = (forward + Vector3.up * 0.75f).normalized;
            if (!Physics.Raycast(transform.GetChild(0).position, rayDirection, out hit, raycastDistance))
            {
                // Pitch Up
                targetPitch = 12.0f;
                return;
            }

            // Can I pitch up and to the side to survive
            rayDirection = (forward + Vector3.up * 0.5f + transform.GetChild(0).right * 0.5f).normalized;
            if (!Physics.Raycast(transform.GetChild(0).position, rayDirection, out hit, raycastDistance))
            {
                // Pitch Up & to the right
                targetPitch = 12.0f;
                targetRoll = 0.9f;
                return;
            }

            rayDirection = (forward + Vector3.up * 0.5f - transform.GetChild(0).right * 0.5f).normalized;
            if (!Physics.Raycast(transform.GetChild(0).position, rayDirection, out hit, raycastDistance))
            {
                // Pitch Up & to the left
                targetPitch = 12.0f;
                targetRoll = -0.9f;
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_physics == null || !_enabled) return;

        Vector3 forward = _physics._velocity.normalized;
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.GetChild(0).position + transform.GetChild(0).right * 2.7f, transform.GetChild(0).position + transform.GetChild(0).right * 2.7f + forward * (ODDistance + _physics._horizontalVelocity.magnitude * ODHorizontalVelMult));
        Gizmos.DrawLine(transform.GetChild(0).position - transform.GetChild(0).right * 2.7f, transform.GetChild(0).position - transform.GetChild(0).right * 2.7f + forward * (ODDistance + _physics._horizontalVelocity.magnitude * ODHorizontalVelMult));
    }
}
