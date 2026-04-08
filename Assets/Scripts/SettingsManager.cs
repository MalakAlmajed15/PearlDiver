using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Light directionalLight;
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public GameObject settingsPanel;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        float savedBrightness = PlayerPrefs.GetFloat("brightness", directionalLight.intensity);

        volumeSlider.value = savedVolume;
        brightnessSlider.value = savedBrightness;

        SetVolume(savedVolume);
        SetBrightness(savedBrightness);
    }

    public void OnSettingsOpen()
    {
        settingsPanel.SetActive(true);
        // Call this when the Settings button is clicked
        float currentVol;
        mainMixer.GetFloat("MyExposedVolume", out currentVol);
        // Convert dB back to 0-1
        volumeSlider.value = Mathf.Pow(10, currentVol / 20);
        brightnessSlider.value = directionalLight.intensity;
    }

    public void SetVolume(float sliderValue)
    {
        float normalized = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("MyExposedVolume", Mathf.Log10(normalized) * 20);

        PlayerPrefs.SetFloat("volume", sliderValue); // 💾 save
    }

    public void SetBrightness(float sliderValue)
    {
        if (directionalLight != null)
            directionalLight.intensity = sliderValue;

        PlayerPrefs.SetFloat("brightness", sliderValue); // 💾 save
    }
}