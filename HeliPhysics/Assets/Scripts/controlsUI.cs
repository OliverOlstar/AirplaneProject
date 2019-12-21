using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlsUI : MonoBehaviour
{
    [SerializeField] private GameObject toggleUI;
    [SerializeField] private KeyCode button;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(button))
        {
            toggleUI.SetActive(!toggleUI.activeSelf);
        }
    }
}
