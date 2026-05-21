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

    [Header("Hit Sound")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float hitSoundVolume = 1f;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathSoundVolume = 1f;
    private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool enableDebugDamageKey = true;
    [SerializeField] private KeyCode debugDamageKey = KeyCode.K;

    private int currentHealth;
    private bool isInvincible = false;
    private bool isDead = false;
    private Coroutine invincibilityRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();

        // Set up audio source for hit sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound

        if ((renderersToFlash == null || renderersToFlash.Length == 0) && modelObject != null)
            renderersToFlash = modelObject.GetComponentsInChildren<Renderer>();

        Debug.Log("Player health initialized: " + currentHealth);
    }

    private void Update()
    {
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

        if (isInvincible)
        {
            Debug.Log("TakeDamage ignored: player is invincible.");
            return;
        }

        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);

        // Play hit sound when diver gets hit
        PlayHitSound();

        if (UIManager.Instance != null)
            UIManager.Instance.LoseLife();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        invincibilityRoutine = StartCoroutine(InvincibilityCoroutine());
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, hitSoundVolume);
            Debug.Log("Hit sound played!");
        }
    }

    private void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolume);
            Debug.Log("Death sound played!");
        }
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

        while (timer < invincibilityDuration)
        {
            ToggleFlashRenderers(false);
            yield return new WaitForSeconds(flashInterval);

            ToggleFlashRenderers(true);
            yield return new WaitForSeconds(flashInterval);

            timer += flashInterval * 2f;
        }

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

        // Play death sound
        PlayDeathSound();

        if (invincibilityRoutine != null)
            StopCoroutine(invincibilityRoutine);

        ToggleFlashRenderers(true);

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

        if (collidersToDisableOnDeath != null)
        {
            foreach (Collider col in collidersToDisableOnDeath)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

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

        enabled = false;
    }

    public int GetCurrentHealth() { return currentHealth; }
    public bool IsDead() { return isDead; }
    public bool IsInvincible() { return isInvincible; }
}