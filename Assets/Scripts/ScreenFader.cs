using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;
    public bool IsFullyBlack()
    {
        return fadeImage != null && fadeImage.color.a >= 0.95f; // ✅ instead of 1f
    }

    public bool IsFullyClear()
    {
        return fadeImage != null && fadeImage.color.a <= 0.05f; // ✅ instead of 0f
    }
    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    void Start()
    {
        SetAlpha(0f); // Start fully transparent
    }

    void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime / fadeDuration);
        SetAlpha(currentAlpha);
    }

    void SetAlpha(float a)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = a;
            fadeImage.color = color;
        }
    }

    public void FadeToBlack()
    {
        targetAlpha = 1f;
    }

    public void FadeToClear()
    {
        targetAlpha = 0f;
    }
}
