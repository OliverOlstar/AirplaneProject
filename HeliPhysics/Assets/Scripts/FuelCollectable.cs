using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCollectable : MonoBehaviour
{
    [SerializeField] private float fuel = 60;
    [SerializeField] private float restoreDelay = 5;
    private MeshRenderer _renderer;
    private SphereCollider _collider;
    private Light _light;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<SphereCollider>();
        _light = GetComponent<Light>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Fuel otherFuel = other.GetComponentInParent<Fuel>();

        if (otherFuel)
        {
            otherFuel.ModifyFuel(fuel);
            ToggleComponents(false);
            StartCoroutine("RestoreRoutine");
        }
    }

    private IEnumerator RestoreRoutine()
    {
        yield return new WaitForSeconds(restoreDelay);
        ToggleComponents(true);
    }

    private void ToggleComponents(bool pActive)
    {
        _renderer.enabled = pActive;
        _collider.enabled = pActive;
        _light.enabled = pActive;
    }
}
