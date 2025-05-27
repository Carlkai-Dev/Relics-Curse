using UnityEngine;

public class RiverAudioTrigger : MonoBehaviour
{

    [SerializeField] private AudioSource riverAudio; // ✅ also visible

    public float fadeSpeed = 1f; // how fast it fades

    private bool playerInside = false;

    void Start()
    {
        if (riverAudio != null)
        {
            riverAudio.volume = 0f;
            riverAudio.Stop();
        }
    }

    void Update()
    {
        if (riverAudio == null) return;

        // Fade in or out
        if (playerInside)
        {
            if (!riverAudio.isPlaying)
                riverAudio.Play();

            riverAudio.volume = Mathf.MoveTowards(riverAudio.volume, 1f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            riverAudio.volume = Mathf.MoveTowards(riverAudio.volume, 0f, fadeSpeed * Time.deltaTime);
            if (riverAudio.volume <= 0.01f && riverAudio.isPlaying)
                riverAudio.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
