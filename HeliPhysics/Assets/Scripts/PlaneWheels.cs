using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWheels : MonoBehaviour
{
    private PlanePhysics physics;
    [SerializeField] private Vector3 raycastDirection;
    [SerializeField] private float raycastLength;
    [SerializeField] private Vector3 raycastStartOffset;
    [SerializeField] [Range(0, 0.5f)] private float raycastExtraLength;
    [SerializeField] private float raycastExtraMult;
    [SerializeField] private float raycastMult;

    [SerializeField] private float coefficientOfFriction = 0.4f;
    [SerializeField] [Range(0, 1)] private float bounciness;

    [Space]
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float rotationMult;

    [SerializeField] private bool Debugs = false;

    void Start()
    {
        physics = GetComponentInParent<PlanePhysics>();
    }

    public void CheckWithWheel()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + raycastStartOffset, raycastDirection.normalized, out hit, raycastLength))
        {
            //Normal Force
            Vector3 normalForce = CalculateNormal(hit);

            //Friction
            //normalForce += CalculateFriction();

            physics._angluarVelocity += rotation * normalForce.magnitude * Time.deltaTime * rotationMult;
            if (Debugs) Debug.Log(normalForce.magnitude * Time.deltaTime * rotationMult);
            physics._velocity += normalForce;
        }
    }

    private Vector3 CalculateNormal(RaycastHit pHit)
    {
        float normalMult = bounciness + -pHit.distance + raycastExtraLength;
        if (normalMult < 0)
        {
            normalMult = (normalMult + 1) * raycastExtraMult;
        }
        else
        {
            normalMult = (normalMult * raycastMult) + 1;
        }

        Vector3 normalForce = Vector3.Project(physics._velocity, pHit.normal) * normalMult;
        return -normalForce;
    }

    private Vector3 CalculateFriction()
    {
        Vector3 friction = coefficientOfFriction * physics._velocity;
        if (friction.magnitude <= 0.002f) 
            return physics._velocity;
        else
            return friction;

        // TODO Add Cut off at small enough number
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + raycastStartOffset, transform.position + raycastStartOffset + (raycastDirection.normalized * raycastLength));
        Gizmos.DrawSphere(transform.position + raycastStartOffset + (raycastDirection.normalized * raycastExtraLength), 0.03f);
    }
}
