using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crashDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(transform.parent.gameObject, 4.0f);
    }
}
