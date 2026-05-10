using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    private AirMeterController airMeter;

    void Start()
    {
        // Find the diver and get the AirMeterController
        GameObject diver = GameObject.FindWithTag("Player");
        if (diver != null)
        {
            airMeter = diver.GetComponent<AirMeterController>();
        }
    }

    // Diver enters water — start depleting air
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (airMeter != null)
            {
                airMeter.isDepleting = true;
                Debug.Log("Diver entered water — air depleting!");
            }
        }
    }

    // Diver exits water — stop depleting and refill
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (airMeter != null)
            {
                airMeter.isDepleting = false;
                airMeter.RefillAir();
                Debug.Log("Diver exited water — air refilled!");
            }
        }
    }
}