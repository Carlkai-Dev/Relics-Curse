using UnityEngine;

public class UIFlicker : MonoBehaviour
{
    public float flickerSpeed = 2f;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // Alpha oscillates from 0 to 1 like a sine wave
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * flickerSpeed));
        canvasGroup.alpha = alpha;
    }
}
