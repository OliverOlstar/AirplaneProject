using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWheelsRotation : MonoBehaviour
{
    private Vector3[] targetTriangleNormals = new Vector3[3];
    private Vector3 targetAverageNormal;


    public void SetTriangle(int pIndex, Vector3 pNormal)
    {
        targetTriangleNormals[pIndex] = pNormal;
    }

    // Update is called once per frame
    void Update()
    {
        targetAverageNormal = (targetTriangleNormals[0] + targetTriangleNormals[1] + targetTriangleNormals[2]) / 3;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + targetAverageNormal);
    }
}
