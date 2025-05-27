// QuestItemPickup.cs
// Handles logic when player picks up the quest orb item
// NOTES (future Carlkai):
// - Calls QuestGiver to update quest stage
// - Notifies RespawnManager that the orb was collected
// - Plays optional pickup sound
// - Destroys itself after pickup

using UnityEngine;

public class QuestItemPickup : MonoBehaviour
{
    [Header("References")]
    public QuestGiver questGiver; // Set in Inspector

    [Header("Audio")]
    public AudioClip pickupSound; // Optional sound on pickup
    public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Inform QuestGiver to update quest stage
            if (questGiver != null)
            {
                questGiver.MarkOrbCollected();
            }

            // Update respawn behavior (orb checkpoint)
            RespawnManager manager = FindObjectOfType<RespawnManager>();
            if (manager != null)
            {
                manager.SetOrbCollected();
            }

            // Play sound effect at pickup location
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
            }

            // Remove the orb from the scene
            Destroy(gameObject);
        }
    }
}