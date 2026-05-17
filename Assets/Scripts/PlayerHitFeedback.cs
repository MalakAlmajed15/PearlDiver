using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Flash Settings")]
    public Renderer playerRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.2f;

    private Color originalColor;

    void Start()
    {
        if (playerRenderer == null)
            playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;
    }

    public void PlayHitFeedback()
    {
        StartCoroutine(FlashRed());
    }

    System.Collections.IEnumerator FlashRed()
    {
        if (playerRenderer != null)
            playerRenderer.material.color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        if (playerRenderer != null)
            playerRenderer.material.color = originalColor;
    }
}