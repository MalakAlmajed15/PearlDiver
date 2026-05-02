using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeMagnitude = 0.08f;

    [Header("Screen Flash")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashAlpha = 0.25f;
    [SerializeField] private float flashFadeSpeed = 6f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSfx;

    private Vector3 cameraLocalStartPos;
    private Coroutine shakeRoutine;

    private void Start()
    {
        if (cameraTransform != null)
            cameraLocalStartPos = cameraTransform.localPosition;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayHitFeedback()
    {
        if (hurtSfx != null && audioSource != null)
            audioSource.PlayOneShot(hurtSfx);

        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = flashAlpha;
            flashImage.color = c;
        }

        if (cameraTransform != null)
        {
            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);

            shakeRoutine = StartCoroutine(CameraShakeRoutine());
        }
    }

    private void Update()
    {
        if (flashImage != null)
        {
            Color c = flashImage.color;
            if (c.a > 0f)
            {
                c.a = Mathf.MoveTowards(c.a, 0f, flashFadeSpeed * Time.deltaTime);
                flashImage.color = c;
            }
        }
    }

    private IEnumerator CameraShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            Vector3 offset = Random.insideUnitSphere * shakeMagnitude;
            offset.z = 0f;

            cameraTransform.localPosition = cameraLocalStartPos + offset;
            yield return null;
        }

        cameraTransform.localPosition = cameraLocalStartPos;
        shakeRoutine = null;
    }
}