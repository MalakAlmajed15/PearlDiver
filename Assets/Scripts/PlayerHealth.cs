using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityDuration = 2f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("Visual Flash")]
    [SerializeField] private GameObject modelObject;
    [SerializeField] private Renderer[] renderersToFlash;

    [Header("Death Setup")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnDeath;
    [SerializeField] private Collider[] collidersToDisableOnDeath;
    [SerializeField] private GameObject[] objectsToDisableOnDeath;
    [SerializeField] private GameObject gameOverUI;

    [Header("Optional Visual Death")]
    [SerializeField] private bool disableRendererOnDeath = false;
    [SerializeField] private Renderer[] renderersToDisableOnDeath;

    [Header("Knockback")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float knockbackForceMultiplier = 1f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugDamageKey = true;
    [SerializeField] private KeyCode debugDamageKey = KeyCode.K;

    private int currentHealth;
    private bool isInvincible = false;
    private bool isDead = false;
    private Coroutine invincibilityRoutine;

    private void Awake()
    {
        // Simple global access point for systems that need to check player health.
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();

        // Auto-fill renderers from the assigned model object if no custom list was provided.
        if ((renderersToFlash == null || renderersToFlash.Length == 0) && modelObject != null)
            renderersToFlash = modelObject.GetComponentsInChildren<Renderer>();

        Debug.Log("Player health initialized: " + currentHealth);
    }

    private void Update()
    {
        // Optional play-mode shortcut for quickly testing damage, UI updates, and death logic.
        if (enableDebugDamageKey && Input.GetKeyDown(debugDamageKey))
            TakeDamage(1);
    }

    public void TakeDamage()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        // Invincibility frames prevent enemies from draining all health in one contact.
        if (isInvincible)
        {
            Debug.Log("TakeDamage ignored: player is invincible.");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        if (UIManager.Instance != null)
            UIManager.Instance.LoseLife();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        // Restarting the coroutine avoids overlapping flash/invincibility routines.
        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        invincibilityRoutine = StartCoroutine(InvincibilityCoroutine());
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (playerRigidbody == null || isDead)
            return;

        playerRigidbody.AddForce(
            direction.normalized * force * knockbackForceMultiplier,
            ForceMode.Impulse
        );
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        Debug.Log("Player is invincible.");

        float timer = 0f;

        // Flash the visible model while the player is temporarily immune to damage.
        while (timer < invincibilityDuration)
        {
            ToggleFlashRenderers(false);
            yield return new WaitForSeconds(flashInterval);

            ToggleFlashRenderers(true);
            yield return new WaitForSeconds(flashInterval);

            timer += flashInterval * 2f;
        }

        // Always restore visibility, even if the final flash ended on an invisible frame.
        ToggleFlashRenderers(true);
        isInvincible = false;
        invincibilityRoutine = null;

        Debug.Log("Player invincibility ended.");
    }

    private void ToggleFlashRenderers(bool state)
    {
        if (renderersToFlash == null)
            return;

        foreach (Renderer rend in renderersToFlash)
        {
            if (rend != null)
                rend.enabled = state;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Debug.Log("PLAYER DIED");

        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        // Ensure the model is not left hidden by the invincibility flash.
        ToggleFlashRenderers(true);

        // Disable player control, camera control, or any other gameplay scripts assigned in the Inspector.
        if (scriptsToDisableOnDeath != null)
        {
            foreach (MonoBehaviour script in scriptsToDisableOnDeath)
            {
                if (script != null)
                {
                    script.enabled = false;
                    Debug.Log("Disabled script: " + script.GetType().Name);
                }
            }
        }

        // Disable colliders so the dead player no longer receives enemy/contact interactions.
        if (collidersToDisableOnDeath != null)
        {
            foreach (Collider col in collidersToDisableOnDeath)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        // Optional object disabling for cameras, UI helpers, movement rigs, or visual elements.
        if (objectsToDisableOnDeath != null)
        {
            foreach (GameObject obj in objectsToDisableOnDeath)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log("Disabled object: " + obj.name);
                }
            }
        }

        // Allows the death state to hide the character model if the project needs that behavior.
        if (disableRendererOnDeath && renderersToDisableOnDeath != null)
        {
            foreach (Renderer rend in renderersToDisableOnDeath)
            {
                if (rend != null)
                    rend.enabled = false;
            }
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Debug.Log("Game Over UI shown.");
        }

        // This component no longer needs to process input or damage after death.
        enabled = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}