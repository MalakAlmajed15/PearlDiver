using UnityEngine;

public class UnderwaterFog : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            RenderSettings.fog = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            RenderSettings.fog = false;
        }
    }
}