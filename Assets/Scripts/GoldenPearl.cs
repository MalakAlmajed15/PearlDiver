using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenPearl : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Give the player an extra life
            AirMeterController airMeter = other.GetComponent<AirMeterController>();
            if (airMeter != null)
            {
                airMeter.lives++;
                Debug.Log("Extra life collected! Lives: " + airMeter.lives);
            }

            // Destroy the golden pearl after collection
            Destroy(gameObject);
        }
    }
}