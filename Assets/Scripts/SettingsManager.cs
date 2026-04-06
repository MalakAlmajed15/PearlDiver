using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Light directionalLight;

    public void SetVolume(float sliderValue)
    {
        float normalized = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("MyExposedVolume", Mathf.Log10(normalized) * 20);
    }

    public void SetBrightness(float sliderValue)
    {
        // Slider 0-100, scale to a reasonable intensity range
        if (directionalLight != null)
            directionalLight.intensity = sliderValue / 100f * 3f; // 0 to 3 intensity
    }
}