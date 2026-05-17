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

    [Header("Lives")]
    public int lives = 3;

    private float currentAir;
    public bool isDepleting = false;

    void Start()
    {
        currentAir = maxAir;
        airSlider.maxValue = maxAir;
        airSlider.value = maxAir;
    }

    void Update()
    {
        
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
            LoseLife();
        }
    }

    public void RefillAir()
    {
        currentAir = maxAir;
        airSlider.value = maxAir;
        isDepleting = true;
        Debug.Log("Air refilled!");
    }

    void LoseLife()
    {
        lives--;
        Debug.Log("Lost a life! Lives left: " + lives);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoseLife();
        }

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(FlashRed());
            Invoke("RefillAir", 1f); // Still used for the "penalty refill" after death
        }
    }

    IEnumerator FlashRed()
    {
        Debug.Log("Life lost!");
        yield return new WaitForSeconds(0.5f);
    }
}