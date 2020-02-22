using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private int collectableIndex = 0;
    [SerializeField] private Vector3 rotation;
    private bool collectedAlready = false;

    private void Start()
    {
        CollectableScore.Instance.addToMax(collectableIndex);
        transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
    }

    private void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && collectedAlready == false)
        {
            CollectableScore.Instance.Collected(collectableIndex);
            collectedAlready = true;
            Destroy(gameObject);
        }
    }
}
