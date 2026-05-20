using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Applies underwater fog, post processing, and bubble effects.
/// Attach to the Main Camera.
/// </summary>
[RequireComponent(typeof(Camera))]
public class UnderwaterVisuals : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private UnderwaterSwimController playerController;
    [SerializeField] private bool includeCameraPosition = true;
    [SerializeField] private float cameraCheckRadius = 0.1f;

    [Header("Fog Settings")]
    [SerializeField] private bool adjustFog = true;

    // Shader color: #3D6E7A
    [SerializeField]
    private Color underwaterFogColor =
        new Color32(0x3D, 0x6E, 0x7A, 255);

    [SerializeField] private FogMode underwaterFogMode = FogMode.Linear;

    // Where fog begins
    [SerializeField] private float underwaterFogStartDistance = 6f;

    // Full fog distance
    [SerializeField] private float underwaterFogEndDistance = 35f;

    [SerializeField] private float transitionSpeed = 2f;

    [Header("Post Processing")]
    [SerializeField] private Volume underwaterVolume;

    [Header("Bubble Effects")]
    [SerializeField] private bool spawnBubbleBursts = true;
    [SerializeField] private ParticleSystem bubbleParticleSystem;

    private bool originalFogEnabled;
    private Color originalFogColor;
    private float originalFogDensity;
    private FogMode originalFogMode;

    private float originalFogStartDistance;
    private float originalFogEndDistance;

    private float currentBlend;
    private float originalVolumeWeight;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<UnderwaterSwimController>();
        }

        if (underwaterVolume != null)
        {
            originalVolumeWeight = underwaterVolume.weight;
        }

        CacheFogSettings();
    }

    private void OnDisable()
    {
        RestoreFogSettings();

        if (underwaterVolume != null)
        {
            underwaterVolume.weight = originalVolumeWeight;
        }

        currentBlend = 0f;
    }

    private void Update()
    {
        bool underwater = DetermineUnderwaterState();

        float targetBlend = underwater ? 1f : 0f;

        currentBlend = Mathf.MoveTowards(
            currentBlend,
            targetBlend,
            transitionSpeed * Time.deltaTime
        );

        if (adjustFog)
        {
            ApplyFogBlend();
        }

        if (underwaterVolume != null)
        {
            underwaterVolume.weight =
                Mathf.Lerp(originalVolumeWeight, 1f, currentBlend);
        }
    }

    private bool DetermineUnderwaterState()
    {
        bool underwater = false;

        if (playerController != null)
        {
            if (
                playerController.ClampToWorldHeight &&
                transform.position.y >= playerController.MaxWorldHeight + 1f
            )
            {
                return false;
            }

            underwater |= playerController.IsInWater;
        }

        if (includeCameraPosition)
        {
            underwater |= WaterVolume.IsPointInside(
                transform.position,
                cameraCheckRadius
            );
        }

        return underwater;
    }

    private void CacheFogSettings()
    {
        originalFogEnabled = RenderSettings.fog;
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogMode = RenderSettings.fogMode;

        originalFogStartDistance = RenderSettings.fogStartDistance;
        originalFogEndDistance = RenderSettings.fogEndDistance;
    }

    private void ApplyFogBlend()
    {
        if (currentBlend <= 0f)
        {
            RestoreFogSettings();
            return;
        }

        RenderSettings.fog = true;

        // Use linear fog for smooth underwater depth
        RenderSettings.fogMode = FogMode.Linear;

        // Match water shader color
        RenderSettings.fogColor = Color.Lerp(
            originalFogColor,
            underwaterFogColor,
            currentBlend
        );

        // Smooth fog start distance
        RenderSettings.fogStartDistance = Mathf.Lerp(
            originalFogStartDistance,
            underwaterFogStartDistance,
            currentBlend
        );

        // Smooth fog end distance
        RenderSettings.fogEndDistance = Mathf.Lerp(
            originalFogEndDistance,
            underwaterFogEndDistance,
            currentBlend
        );
    }

    private void RestoreFogSettings()
    {
        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogMode = originalFogMode;

        RenderSettings.fogStartDistance = originalFogStartDistance;
        RenderSettings.fogEndDistance = originalFogEndDistance;
    }
}