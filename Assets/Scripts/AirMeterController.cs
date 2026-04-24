using UnityEngine;
using UnityEngine.UI;

public class AirMeterController : MonoBehaviour
{
    [Header("Air Settings")]
    public float maxAir = 100f;
    public float airDepletionRate = 5f;

    [Header("UI")]
    public Slider airSlider;

    [Header("Lives")]
    public int lives = 3;

    private float currentAir;
    private bool isDepleting = true;

    void Start()
    {
        currentAir = maxAir;
        airSlider.maxValue = maxAir;
        airSlider.value = maxAir;
    }

    void Update()
    {
        if (isDepleting)
        {
        
            currentAir -= airDepletionRate * Time.deltaTime;
            currentAir = Mathf.Clamp(currentAir, 0, maxAir);
            airSlider.value = currentAir;

        
            if (currentAir <= 0)
            {
                isDepleting = false;
                LoseLife();
            }
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

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
          
        }
        else
        {
            Invoke("RefillAir", 1f);
        }
    }
}