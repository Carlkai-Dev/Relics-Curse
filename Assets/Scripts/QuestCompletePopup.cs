// QuestCompletePopup.cs
// Controls animated pop-up when the quest is completed
// NOTES (future Carlkai):
// - Shows "QUEST" then "COMPLETE" images with delay between
// - Plays audio on popup
// - Fades out automatically after a set duration

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuestCompletePopup : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // Controls popup visibility
    public float fadeDuration = 1f; // Duration of fade-out
    public float showTime = 2f;     // Time before auto-hide

    [Header("Quest Complete FX")]
    public GameObject questImage;      // Image that says "QUEST"
    public GameObject completeImage;   // Image that says "COMPLETE"
    public float delayBetween = 0.7f;  // Time between showing both images

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        canvasGroup.alpha = 1f;

        if (questImage != null) questImage.SetActive(false);
        if (completeImage != null) completeImage.SetActive(false);

        if (audioSource != null)
            audioSource.Play();

        StartCoroutine(ShowQuestCompleteSequence());

        CancelInvoke();
        Invoke(nameof(HidePopup), showTime);
    }

    IEnumerator ShowQuestCompleteSequence()
    {
        if (questImage != null)
            questImage.SetActive(true);

        yield return new WaitForSeconds(delayBetween);

        if (completeImage != null)
            completeImage.SetActive(true);
    }

    void HidePopup()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
