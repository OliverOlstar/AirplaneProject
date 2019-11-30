using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWheels : MonoBehaviour
{
    private PlanePhysics physics;
    [SerializeField] private Vector3 raycastDirection;
    [SerializeField] private float raycastLength;
    [SerializeField] private Vector3 raycastOffset;

    [SerializeField] private float coefficientOfFriction = 0.4f;
    [SerializeField] [Range(0, 1)] private float bounciness;

    [SerializeField] private float bounceBuffer;

    [Space]
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float rotationMult;
    [SerializeField] private float rotationMin = 5;

    void Start()
    {
        physics = GetComponentInParent<PlanePhysics>();
    }

    public void CheckWithWheel()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + raycastOffset, raycastDirection.normalized, out hit, raycastLength))
        {
            //Normal Force
            physics._velocity += CalculateNormal(hit);

            //Friction
            Vector3 normalForce = CalculateFriction();

            physics._angluarVelocity += rotation * normalForce.magnitude * Mathf.Max(rotationMin, physics._horizontalVelocity.magnitude) * Time.deltaTime * rotationMult;
            physics._velocity -= normalForce * Time.deltaTime;

            if (physics._velocity.y < bounceBuffer)
                physics._velocity.y = Mathf.Epsilon;
        }
    }

    private Vector3 CalculateNormal(RaycastHit pHit)
    {
        float normalMult = bounciness + 1 + Mathf.Pow(raycastLength - pHit.distance, 3);
        //if (normalMult <= bounceBuffer) normalMult -= bounciness;

        Vector3 normalForce = Vector3.Project(physics._velocity, pHit.normal) * normalMult;
        if (Vector3.Dot(pHit.normal, normalForce) < 0)
            return -normalForce;

        return Vector3.zero;
    }

    private Vector3 CalculateFriction()
    {
        return coefficientOfFriction * physics._velocity;

        // TODO Add Cut off at small enough number
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + raycastOffset, transform.position + raycastOffset + raycastDirection.normalized * raycastLength);
    }
}
