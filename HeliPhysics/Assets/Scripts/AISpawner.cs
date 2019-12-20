using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnRateVariation;

    [SerializeField] private GameObject planeAIPrefab;

    [SerializeField] private bool onlyOne;

    private LandingStrip[] landingStrips;

    // Start is called before the first frame update
    void Start()
    {
        landingStrips = FindObjectsOfType<LandingStrip>();
        StartCoroutine("SpawnRoutine");
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate + Random.Range(-spawnRateVariation, spawnRateVariation));

            int index = Mathf.RoundToInt(Random.value * (landingStrips.Length - 1));
            LandingStrip A = landingStrips[index];
            index = Mathf.RoundToInt(Random.value * (landingStrips.Length - 1));
            LandingStrip B = landingStrips[index];

            if (A == B)
            {
                index++;
                if (index == landingStrips.Length) 
                    index = 0;

                B = landingStrips[index];
            }

            SpawnAIPlane(A, B);

            if (onlyOne) break;
        }
    }

    private void SpawnAIPlane(LandingStrip pPointA, LandingStrip pPointB)
    {
        // Spawn new plane
        GameObject plane = Instantiate(planeAIPrefab);

        // Position Plane
        Vector3 spawnPosition = pPointA.transform.position + -pPointA.transform.forward * pPointA.stripLength / 2.1f - pPointA.transform.right * pPointA.stripWidth / 2 * Random.Range(-1.4f, 1.4f);

        plane.transform.GetChild(0).position = spawnPosition + Vector3.up * 3;
        plane.transform.GetChild(0).rotation = Quaternion.Euler(-4, pPointA.transform.eulerAngles.y + Random.Range(-50, 50), 0);

        // Set Variables
        plane.GetComponent<TakeOff>().pointA = pPointA;
        plane.GetComponent<FlyToTarget>().pointB = pPointB;
        plane.GetComponent<Landing>().pointB = pPointB;
    }
}
