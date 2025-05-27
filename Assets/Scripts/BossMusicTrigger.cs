using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    public AudioSource bossMusicSource; // 🎵 Drag your music AudioSource here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossMusicSource != null && !bossMusicSource.isPlaying)
            {
                bossMusicSource.Play();
            }

            // Optional: Disable the trigger so it only plays once
            gameObject.SetActive(false);
        }
    }
}
