using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizomoPlanes : MonoBehaviour
{
    [SerializeField] private PlanePhysics _physics;
    [SerializeField] private float _drawLength;
    [SerializeField] private bool _drawLiftPath;

    private void OnDrawGizmos()
    {
        if (_physics)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _physics._gravity * _drawLength);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _physics._verticalVelocity * _drawLength);
            Gizmos.DrawLine(transform.position, transform.position + _physics._horizontalVelocity * _drawLength);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _physics._lift * _drawLength);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + _physics.thrust * transform.forward * _drawLength);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + _physics._drag);

            if (_drawLiftPath)
            {
                Gizmos.color = Color.magenta;
                Vector3 pos = transform.position;
                for (float i = 1; i <= 20; i++)
                {
                    Vector3 nextPos = pos + Vector3.forward / 2;
                    nextPos.y = _physics.GetQuadraticCurveValue(i / 20f) + transform.position.y;
                    Gizmos.DrawSphere(nextPos, 0.1f);
                    Gizmos.DrawLine(pos, nextPos);
                    pos = nextPos;
                }

                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 10);
                Gizmos.DrawLine(transform.position + 0.5f * Vector3.up, transform.position + Vector3.forward * 10 + 0.5f * Vector3.up);
                Gizmos.DrawLine(transform.position + 5 * Vector3.forward, transform.position + Vector3.forward * 5 + 2 * Vector3.up);
            }
        }
    }
}
