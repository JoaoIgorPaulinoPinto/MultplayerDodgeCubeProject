using System.Collections;
using UnityEngine;

public class PlayerEffectsManager : MonoBehaviour
{
    [SerializeField] private Color pointColor = Color.green;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private Color defaultColor;
    [SerializeField] private float effectDuration = 0.33f;

    private Renderer playerRenderer;

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<MeshRenderer>();
        if (playerRenderer == null)
        {
            Debug.LogError("Player renderer not found!");
        }
    }

    public void PlayPointEffect()
    {
        StartCoroutine(FlashColor(pointColor));
    }

    public void PlayHitEffect()
    {
        StartCoroutine(FlashColor(hitColor));
    }
    private void Start()
    {
        defaultColor = playerRenderer.material.color;
    }
    private IEnumerator FlashColor(Color targetColor)
    {
        Color initialColor = playerRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            // Interpola entre a cor inicial e a cor do efeito
            playerRenderer.material.color = Color.Lerp(initialColor, targetColor, Mathf.PingPong(elapsedTime / (effectDuration / 2), 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restaura a cor original
        playerRenderer.material.color = initialColor;
    }
}
