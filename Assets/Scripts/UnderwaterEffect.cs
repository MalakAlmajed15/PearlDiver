using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterEffect : MonoBehaviour
{
    [Header("Underwater Settings")]
    public Color underwaterFogColor = new Color(0.0f, 0.4f, 0.6f, 1f);
    public float fogDensity = 0.05f;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = fogDensity;

        // Tint the background/skybox blue-green
        RenderSettings.ambientLight = new Color(0.0f, 0.3f, 0.5f);
    }
}