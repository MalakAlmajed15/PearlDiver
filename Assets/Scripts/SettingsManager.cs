using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public GameObject settingsPanel;

    [Header("Brightness Overlay")]
    public Image brightnessOverlay; 

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        float savedBrightness = PlayerPrefs.GetFloat("brightness", 0.5f);

        volumeSlider.value = savedVolume;
        brightnessSlider.value = savedBrightness;

        SetVolume(savedVolume);
        SetBrightness(savedBrightness);
    }

    public void OnSettingsOpen()
    {
        settingsPanel.SetActive(true);

        float currentVol;
        mainMixer.GetFloat("MyExposedVolume", out currentVol);
        volumeSlider.value = Mathf.Pow(10, currentVol / 20);
        brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 0.5f);
    }

    public void SetVolume(float sliderValue)
    {
        float normalized = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("MyExposedVolume", Mathf.Log10(normalized) * 20);
        PlayerPrefs.SetFloat("volume", sliderValue);
    }

    public void SetBrightness(float sliderValue)
    {

        float overlayAlpha = (1f - sliderValue) * 0.8f;

        Color c = brightnessOverlay.color;
        brightnessOverlay.color = new Color(c.r, c.g, c.b, overlayAlpha);

        PlayerPrefs.SetFloat("brightness", sliderValue);
    }
}