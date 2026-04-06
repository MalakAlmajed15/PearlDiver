using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Light directionalLight;

    public void SetVolume(float sliderValue)
    {
        // Sliders go 0 to 1, but Mixers use Decibels (-80 to 0)
        mainMixer.SetFloat("MyExposedVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetBrightness(float sliderValue)
    {
        // Adjust the intensity of your main light
        if (directionalLight != null)
        {
            directionalLight.intensity = sliderValue;
        }
    }
}