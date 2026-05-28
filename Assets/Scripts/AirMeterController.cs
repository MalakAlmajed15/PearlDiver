using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AirMeterController : MonoBehaviour
{
    [Header("Air Settings")]
    public float maxAir = 100f;
    public float airDepletionRate = 5f;
    public float airRefillRate = 20f;

    [Header("UI")]
    public Slider airSlider;

    [Header("Low Air Warning Effect")]
    public float lowAirPercentage = 0.25f; // Triggers at 25% air

    [Header("Underwater Sound")]
    public AudioClip underwaterAmbientSound;
    private AudioSource audioSource;

    private float currentAir;
    public bool isDepleting = false;
    private bool isRecovering = false;

    void Start()
    {
        currentAir = maxAir;

        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (underwaterAmbientSound != null)
        {
            audioSource.clip = underwaterAmbientSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            audioSource.spatialBlend = 0f; // 2D sound
        }

        
    }

    void Update()
    {
        if (airSlider == null)
        {
            GameObject sliderObj = GameObject.FindWithTag("AirMeterUI");
            if (sliderObj != null)
            {
                airSlider = sliderObj.GetComponent<Slider>();
                airSlider.maxValue = maxAir;
                airSlider.value = currentAir;
            }
            else
            {
                return;
            }
        }

        if (isRecovering) return;

        // Above water surface
        if (transform.position.y >= 8f)
        {
            isDepleting = false;

            // Stop underwater sound at surface
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            if (currentAir < maxAir)
            {
                currentAir += airRefillRate * Time.deltaTime;
                currentAir = Mathf.Clamp(currentAir, 0, maxAir);
                airSlider.value = currentAir;
            }

            // Turn off warning because it is refilling air at the surface
            if (UIManager.Instance != null) UIManager.Instance.ToggleLowAirWarning(false);

            return;
            
        }

        // Underwater — play sound
        if (audioSource != null &&
            underwaterAmbientSound != null &&
            !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        isDepleting = true;
        currentAir -= airDepletionRate * Time.deltaTime;
        currentAir = Mathf.Clamp(currentAir, 0, maxAir);
        airSlider.value = currentAir;

        // Turn the vignette on if air is low, otherwise turn it off
        if (UIManager.Instance != null)
        {
            if (currentAir <= (maxAir * lowAirPercentage))
            {
                UIManager.Instance.ToggleLowAirWarning(true);
            }
            else
            {
                UIManager.Instance.ToggleLowAirWarning(false);
            }
        }

        if (currentAir <= 0)
        {
            isDepleting = false;
            isRecovering = true;
            // Turn off the vignette right as they die so it doesn't overlap Game Over
            if (UIManager.Instance != null) UIManager.Instance.ToggleLowAirWarning(false);
            LoseLife();
        }

         

}

    public void RefillAir()
    {
        currentAir = maxAir;
        if (airSlider != null) airSlider.value = maxAir;
        isDepleting = true;
        isRecovering = false;
        // Ensure vignette turns off upon refill
        if (UIManager.Instance != null) UIManager.Instance.ToggleLowAirWarning(false);
        Debug.Log("Air refilled!");
    }

    void LoseLife()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoseLife();

            if (UIManager.Instance.lives <= 0)
            {
                Debug.Log("Game Over triggered by Air!");

                // Stop sound on game over
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            else
            {
                StartCoroutine(FlashRed());
                Invoke("RefillAir", 0.5f);
            }
        }
    }

    IEnumerator FlashRed()
    {
        Debug.Log("Life lost!");
        yield return new WaitForSeconds(0.5f);
    }
}