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

    [SerializeField] private LandingStrip pointB;
    [SerializeField] private float yawnSpeed;
    [SerializeField] private float rollSpeed = 1;
    [SerializeField] private float rollSpeedParallel = 1;

    [SerializeField] private float targetPitch = 0;
    [SerializeField] [Range(-0.5f, 0.5f)] private float targetRoll = 0;
    [SerializeField] private float returnToStripMinRot = 20;
    [SerializeField] private float rollClamp = 0.25f;

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
        if (Vector3.Distance(transform.GetChild(0).position, pointB.transform.position) < 1000)
        {
            _controller.thrust = false;
        }
        else
        {
        }

        LevelOut();
        StayOnLandingStrip();
    }

    private void MakeParallel()
    {
        float relRotY = transform.GetChild(0).eulerAngles.y - pointB.transform.eulerAngles.y + 180;

        //Rotate to be parallel to the landing strip
        if (relRotY < 0)
        {
            _controller.yawn = -Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward) * yawnSpeed;
        }
        else if (relRotY != 0)
        {
            _controller.yawn = Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward) * yawnSpeed;
        }
    }

    private void TurnToPoint()
    {
        //If outside of runway turn onto runway
        Vector3 directionToPoint = (pointB.transform.position - transform.GetChild(0).position);
        directionToPoint.y = 0;
        directionToPoint.Normalize();
        Debug.DrawLine(transform.GetChild(0).position, transform.GetChild(0).position + directionToPoint);
        transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(directionToPoint), Time.deltaTime);
    }

    private void LevelOut()
    {
        _controller.pitch = (_physics._verticalVelocity.y - targetPitch) * pitchMult;
        Debug.Log(-_physics._angluarVelocity.y + targetRoll);
        _controller.roll = -_physics._angluarVelocity.y + targetRoll;
        _controller.yawn = 0;
    }

    private void StayOnLandingStrip()
    {
        //Rotate to be parallel to the landing strip
        float relRotY = transform.GetChild(0).eulerAngles.y - pointB.transform.eulerAngles.y + 180;

        //If outside of runway turn onto runway
        Vector3 directionBetween = transform.GetChild(0).position - pointB.transform.position;
        float disFromPointA = Vector3.Project(directionBetween, pointB.transform.right).magnitude - (pointB.stripWidth / 2);
        disFromPointA = Mathf.Min(disFromPointA, 500);

        if (disFromPointA > -2)
        {
            float dotRight = Vector3.Dot(directionBetween.normalized, pointB.transform.right);
            float dotLeft = Vector3.Dot(directionBetween.normalized, -pointB.transform.right);

            //Debug.Log(disFromPointA / (500 * rollSpeed));
            Debug.Log(relRotY);
            if (dotRight > dotLeft)
            {
                if (relRotY < returnToStripMinRot)
                    targetRoll = Mathf.Pow(disFromPointA, 2) / (500 * rollSpeed);
                else
                    targetRoll = 0;
            }
            else
            {
                if (relRotY > -returnToStripMinRot)
                    targetRoll = -Mathf.Pow(disFromPointA, 2) / (500 * rollSpeed);
                else
                    targetRoll = 0;
            }
        }
        else
        {
            //Become Parallel
            if (relRotY < 0)
            {
                targetRoll = -Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward) * rollSpeedParallel;
            }
            else if (relRotY != 0)
            {
                targetRoll = Vector3.Dot(transform.GetChild(0).forward, pointB.transform.forward) * rollSpeedParallel;
            }
        }

        targetRoll = Mathf.Clamp(targetRoll, -rollClamp, rollClamp);
    }

    //private void TurnToPoint()
    //{
    //    Vector3 myRight = transform.GetChild(0).right;
    //    myRight.y = 0;
    //    myRight.Normalize();
    //    float dotRight = Vector3.Dot(myRight, directionToPoint);
    //    float dotLeft = Vector3.Dot(-myRight, directionToPoint);

    //    float curZRot = transform.GetChild(0).eulerAngles.z;
    //    if (curZRot > 180) curZRot -= 360;

    //    float targetZRot = (dotRight - dotLeft) / 4 * 180;
    //    if (targetZRot > 0) targetZRot = Mathf.Max(targetZRot - 1, -1);
    //    else if (targetZRot < 0) targetZRot = Mathf.Min(targetZRot + 1, 1);
    //    targetZRot = Mathf.Clamp(targetZRot, -80, 80);

    //    Debug.Log(targetZRot);
    //    //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetZRot);
    //    _controller.roll = rollSpeed * Time.deltaTime * -targetZRot;
    //}
}
