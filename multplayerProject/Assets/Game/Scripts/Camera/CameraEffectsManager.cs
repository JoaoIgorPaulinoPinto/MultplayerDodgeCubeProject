using System.Collections;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    [SerializeField] private Color pointColor = Color.green;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private float effectDuration = 0.33f;
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float shakeDuration = 0.5f;

    private Quaternion originalCameraRotation;

    public void PlayPointEffect(Camera cam)
    {
        StartCoroutine(FlashColor(pointColor, cam));
        StartCoroutine(ShakeCamera(0, 0, cam));
    }

    public void PlayHitEffect(Camera cam)
    {
        StartCoroutine(FlashColor(hitColor, cam));
        StartCoroutine(ShakeCamera(0, 1, cam)); // Modificado para ser diferente do efeito de ponto
    }

    private IEnumerator FlashColor(Color targetColor, Camera cam)
    {
        float elapsedTime = 0f;
        Color initialColor = cam.backgroundColor;

        while (elapsedTime < effectDuration)
        {
            float lerpFactor = Mathf.PingPong(elapsedTime / (effectDuration / 2), 1);
            cam.backgroundColor = Color.Lerp(initialColor, targetColor, lerpFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.backgroundColor = initialColor; // Restaura a cor original
    }

    IEnumerator ShakeCamera(float timeModifyer, float intensityModifyer, Camera cam)
    {
        originalCameraRotation = cam.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration + timeModifyer)
        {
            float x = Random.Range(-shakeIntensity - intensityModifyer, shakeIntensity + intensityModifyer);
            float y = Random.Range(-shakeIntensity - intensityModifyer, shakeIntensity + intensityModifyer);
            float z = Random.Range(-shakeIntensity - intensityModifyer, shakeIntensity + intensityModifyer);

            cam.transform.rotation = originalCameraRotation * Quaternion.Euler(x, y, z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.rotation = originalCameraRotation;
    }
}
