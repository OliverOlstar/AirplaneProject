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
        LevelOut();
        //MakeParallel();
        TurnToPoint();
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
        Vector3 directionToPoint = (transform.GetChild(0).position - pointB.transform.position).normalized;
        float dotRight = Vector3.Dot(transform.GetChild(0).right, directionToPoint);
        float dotLeft = Vector3.Dot(-transform.GetChild(0).right, directionToPoint);

        float curZRot = transform.GetChild(0).eulerAngles.z;
        if (curZRot > 180) curZRot -= 360;

        float targetZRot = (dotLeft - dotRight) / 2 * 180;
        targetZRot = Mathf.Clamp(targetZRot, -70, 70);

        Debug.Log(targetZRot - curZRot);
        _controller.roll = rollSpeed * Time.deltaTime * ;
    }

    private void LevelOut()
    {
        _controller.pitch = _physics._verticalVelocity.y * pitchMult;
        _controller.roll = -_physics._angluarVelocity.y;
        _controller.yawn = 0;
    }
}
