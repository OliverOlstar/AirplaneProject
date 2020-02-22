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

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlane(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (planeCrash.activeSelf == true && resetUI.activeSelf == false)
        {
            resetUI.SetActive(true);
            fuelUI.transform.parent.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.R))
            ClickReset();
    }

    public void ClickReset()
    {
        resetUI.SetActive(false);
        fuelUI.transform.parent.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;

        SpawnPlane(transform.position);
    }

    private void SpawnPlane(Vector3 pPosition)
    {
        // Destroy Previous Plane
        if (plane != null) Destroy(plane);

        // Spawn new plane
        plane = Instantiate(planePrefab);

        // Position Plane
        plane.transform.position = pPosition;
        plane.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);

        // Set Variables
        plane.GetComponentInChildren<PlaneVisuals>().camera = cameraPivot;
        plane.GetComponentInChildren<Fuel>().fuelBar = fuelUI.GetComponent<RectTransform>();
        cameraFOV.physics = plane.GetComponentInChildren<PlanePhysics>();

        // Get Reference to planeCrash Object
        planeCrash = plane.transform.GetChild(2).gameObject;
    }
}
