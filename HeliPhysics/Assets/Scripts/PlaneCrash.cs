using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCrash : MonoBehaviour
{
    [SerializeField] private GameObject actualPlane;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            GetComponent<PlaneVisuals>()._target = null;
            actualPlane.SetActive(false);
        }
    }
}
