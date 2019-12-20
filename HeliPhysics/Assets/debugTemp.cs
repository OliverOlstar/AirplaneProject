using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugTemp : MonoBehaviour
{
    [SerializeField] private Transform other;
    private void OnDrawGizmosSelected()
    {
        // If outside of runway turn onto runway
        Vector3 directionBetween = transform.position - other.transform.position;

        float relRotY = Vector3.SignedAngle(transform.forward, other.transform.forward, Vector3.up);
        // Debug.Log(relRotY);

        float dotRight = Vector3.Dot(directionBetween.normalized, other.transform.right);
        //float dotLeft = Vector3.Dot(directionBetween.normalized, -transform.right);

        float projection = Vector3.Project(directionBetween, transform.right).magnitude;

        if (dotRight > 0) Debug.Log("Left");
        else Debug.Log("Right");
        //Debug.Log(relRotY);

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }
}
