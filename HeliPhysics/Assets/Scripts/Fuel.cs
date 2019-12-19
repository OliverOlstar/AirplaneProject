using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour
{
    private float fuel;
    [SerializeField] private float maxFuel = 120;
    [SerializeField] private RectTransform fuelBar;

    // Start is called before the first frame update
    void Start()
    {
        fuel = maxFuel;
    }

    // Update is called once per frame
    void Update()
    {
        fuelBar.localScale = new Vector2(fuel / maxFuel, 1);
    }

    public void ModifyFuel(float pValue)
    {
        fuel += pValue;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);
    }

    public float GetFuel()
    {
        return fuel;
    }
}
