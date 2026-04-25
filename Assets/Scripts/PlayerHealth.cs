using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float invincibilityDuration = 2f;
    public float flashInterval = 0.1f;
    public GameObject modelObject; 

    private bool isInvincible = false;
    private Renderer[] renderers;

    public static PlayerHealth Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Get all renderers in the diver (body, fins, etc.)
        renderers = modelObject.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        // DEBUG: Press K to simulate taking damage
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return; // Exit if already hit

        Debug.Log("Player hit!");
        UIManager.Instance.LoseLife();

        // Start the invincibility sequence
        StartCoroutine(BecomeInvincible());
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;

        // Flashing effect loop
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            // Toggle visibility
            ToggleRenderers(false);
            yield return new WaitForSeconds(flashInterval);
            ToggleRenderers(true);
            yield return new WaitForSeconds(flashInterval);

            timer += flashInterval * 2;
        }

        // Ensure player is visible at the end
        ToggleRenderers(true);
        isInvincible = false;
    }

    private void ToggleRenderers(bool state)
    {
        foreach (var r in renderers)
        {
            r.enabled = state;
        }
    }
}