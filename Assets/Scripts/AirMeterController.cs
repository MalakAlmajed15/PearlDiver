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
    public float airRefillRate = 20f; // How fast air refills at surface

    [Header("UI")]
    public Slider airSlider;

    // removed the 'lives' variable from here
    // The UIManager is now in complete control of tracking lives.

    private float currentAir;
    public bool isDepleting = false;

    // NEW: A lock to stop the script from taking 60 lives a second
    private bool isRecovering = false;

    void Start()
    {
        currentAir = maxAir;
        //airSlider.maxValue = maxAir;
        //airSlider.value = maxAir;
    }

    void Update()
    {
        // search for it using the Tag
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
                // Wait safely until the UI Scene is fully loaded
                return;
            }
        }

        // NEW: If the diver is choking/recovering, freeze the air math!
        if (isRecovering) return;

        if (transform.position.y >= 8f)
        {
            isDepleting = false;

            if (currentAir < maxAir)
            {
                currentAir += airRefillRate * Time.deltaTime;
                currentAir = Mathf.Clamp(currentAir, 0, maxAir);
                airSlider.value = currentAir;
            }
            return;
        }

      
        isDepleting = true;

        currentAir -= airDepletionRate * Time.deltaTime;
        currentAir = Mathf.Clamp(currentAir, 0, maxAir);
        airSlider.value = currentAir;

        if (currentAir <= 0)
        {
            isDepleting = false;
            isRecovering = true;
            LoseLife();
        }
    }

    public void RefillAir()
    {
        currentAir = maxAir;
        if (airSlider != null) airSlider.value = maxAir;
        isDepleting = true;
        isRecovering = false;
        Debug.Log("Air refilled!");
    }

    void LoseLife()
    {
        // it tells the UIManager to handle the actual life loss
        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoseLife();

            // it checks the UIManager's lives to see if it's Game Over
            if (UIManager.Instance.lives <= 0)
            {
                Debug.Log("Game Over triggered by Air!");
                // REMOVED the SceneManager.LoadScene from here. 
                // The UIManager will now show the Game Over panel and wait for the player to press restart
            }
            else
            {
                // If we still have lives, flash red and refill the air
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