using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWheels : MonoBehaviour
{
    private PlanePhysics physics;
    [SerializeField] private Vector3 raycastDirection;
    private Vector3 relativeRaycastDirection;
    [SerializeField] private float raycastLength;
    [SerializeField] private Vector3 raycastStartOffset;
    [SerializeField] [Range(0, 1f)] private float raycastExtraLength;
    [SerializeField] private float raycastExtraMult;
    [SerializeField] private float raycastMult;

    [SerializeField] private float coefficientOfFriction = 0.4f;
    [SerializeField] private float maxFriction = 0.4f;
    [SerializeField] private float liftInfluenceReduction = 0.4f;
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
        relativeRaycastDirection = transform.TransformDirection(raycastDirection).normalized;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + raycastStartOffset, relativeRaycastDirection, out hit, raycastLength))
        {
            //Normal Force
            Vector3 normalForce = CalculateNormal(hit);

            //Friction
            normalForce += CalculateFriction() * Time.deltaTime;

            physics._angluarVelocity += rotation * normalForce.magnitude * Time.deltaTime * rotationMult;
            physics._velocity += normalForce;
        }
    }

    private Vector3 CalculateNormal(RaycastHit pHit)
    {
        //If to far from down direction ignore wheel collision (stops sticking to side of wall)
        if (Vector3.Dot(pHit.normal, Vector3.down) > 0.65f) return Vector3.zero;

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
        float liftInfluence = Mathf.Max(0, physics._lift.y - liftInfluenceReduction);
        Vector3 friction = Vector3.Min(coefficientOfFriction * (physics._velocity - liftInfluence * Vector3.up), maxFriction * physics._velocity.normalized);
        if (friction.magnitude <= 0.005f) 
            return -physics._velocity;
        else
            return -friction;

        // TODO Add Cut off at small enough number
    }

    private void OnDrawGizmos()
    {
        if (!Debugs) return;
        relativeRaycastDirection = transform.TransformDirection(raycastDirection).normalized;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + raycastStartOffset, transform.position + raycastStartOffset + (relativeRaycastDirection * raycastLength));
        Gizmos.DrawSphere(transform.position + raycastStartOffset + (relativeRaycastDirection * raycastExtraLength), 0.03f);
    }
}
