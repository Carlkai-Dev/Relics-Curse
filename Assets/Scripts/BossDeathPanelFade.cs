using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathPanelFade : MonoBehaviour
{
    public float delayBeforeFade = 3f;
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private float timer;
    private bool fading = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        timer = delayBeforeFade;
    }

    void Update()
    {
        if (!fading)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                fading = true;
        }
        else
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;
            if (canvasGroup.alpha <= 0f)
                gameObject.SetActive(false); // hide it completely after fade
        }
    }
}