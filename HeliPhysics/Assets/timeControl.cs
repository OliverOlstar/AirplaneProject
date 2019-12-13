using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale += 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale -= 0.1f;
        }
    }
}
