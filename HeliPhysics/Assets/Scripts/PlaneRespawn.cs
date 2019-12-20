using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRespawn : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;
    private GameObject planeCrash;
    private GameObject plane;
    [SerializeField] private GameObject resetUI;
    [SerializeField] private GameObject fuelUI;
    [SerializeField] private CameraPivot cameraPivot;
    [SerializeField] private FOVbySpeed cameraFOV;

    private LandingStrip[] landingStrips;

    // Start is called before the first frame update
    void Start()
    {
        landingStrips = FindObjectsOfType<LandingStrip>();
        SpawnPlane(transform.position, new Vector3(-4, transform.rotation.y + 90, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (planeCrash.activeSelf == true && resetUI.activeSelf == false)
        {
            resetUI.SetActive(true);
            fuelUI.transform.parent.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ClickReset()
    {
        resetUI.SetActive(false);
        fuelUI.transform.parent.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;

        LandingStrip closestStripToCamera = null;
        float closestDistance = 0;
        foreach(LandingStrip strip in landingStrips)
        {
            if (closestStripToCamera == null)
            {
                closestStripToCamera = strip;
                closestDistance = Vector3.Distance(strip.transform.position, cameraPivot.transform.position);
                continue;
            }

            float myDistance = Vector3.Distance(strip.transform.position, cameraPivot.transform.position);
            if (myDistance < closestDistance)
            {
                closestStripToCamera = strip;
                closestDistance = myDistance;
            }
        }

        SpawnPlane(closestStripToCamera.transform.position - closestStripToCamera.transform.forward * closestStripToCamera.stripLength / 2.1f, new Vector3(-4, closestStripToCamera.transform.rotation.y + 90, 0));
    }

    private void SpawnPlane(Vector3 pPosition, Vector3 pRotation)
    {
        // Destroy Previous Plane
        if (plane != null) Destroy(plane);

        // Spawn new plane
        plane = Instantiate(planePrefab);

        // Position Plane
        plane.transform.position = pPosition;
        plane.transform.rotation = Quaternion.Euler(pRotation);

        cameraPivot.transform.rotation = plane.transform.rotation;

        // Set Variables
        plane.GetComponentInChildren<PlaneVisuals>().camera = cameraPivot;
        plane.GetComponentInChildren<Fuel>().fuelBar = fuelUI.GetComponent<RectTransform>();
        cameraFOV.physics = plane.GetComponentInChildren<PlanePhysics>();

        // Get Reference to planeCrash Object
        planeCrash = plane.transform.GetChild(2).gameObject;
    }
}
