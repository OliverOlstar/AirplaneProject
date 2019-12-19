using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCrash : MonoBehaviour
{
    [SerializeField] private GameObject actualPlane;
    [SerializeField] private GameObject crashedPlane;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            GetComponent<PlaneVisuals>()._target = null;

            crashedPlane.transform.position = actualPlane.transform.position;
            crashedPlane.transform.rotation = actualPlane.transform.rotation;

            crashedPlane.SetActive(true);
            actualPlane.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
