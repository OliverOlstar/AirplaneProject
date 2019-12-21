using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingStrip : MonoBehaviour
{
    public float stripLength = 10;
    public float stripWidth = 10;
    [SerializeField] private Color stripColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = stripColor;
        Vector3 disForward = transform.forward * stripLength / 2;
        Vector3 disRight = transform.right * stripWidth / 2;
        Gizmos.DrawLine(transform.position - disForward, transform.position + disForward);

        Gizmos.DrawLine(transform.position - disForward - disRight, transform.position + disForward - disRight);
        Gizmos.DrawLine(transform.position - disForward + disRight, transform.position + disForward + disRight);
        Gizmos.DrawLine(transform.position + disForward - disRight, transform.position + disForward + disRight);
        Gizmos.DrawLine(transform.position - disForward - disRight, transform.position - disForward + disRight);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + transform.up, transform.position + transform.up + transform.forward * 50);
    }
}
