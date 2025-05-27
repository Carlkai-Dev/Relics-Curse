// BossHealth.cs
// Manages the boss's health system, death sequence, and quest progression after defeat.
// NOTES (future Carlkai):
// - Boss takes 1 damage per fireball cast, dies after reaching 0
// - On death: disables wall, triggers quest stage, plays sound, shows death message
// - Also disables all other MonoBehaviours on the boss

using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 4; // Number of casts until boss dies
    private int currentHealth;
    private Animator animator;
    private bool isDead = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("UI")]
    public Slider healthSlider; // HP bar
    public GameObject deathMessageText; // UI message shown after death

    [Header("Path Blocker")]
    public GameObject pathBlocker; // Wall that disappears after boss dies

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (pathBlocker != null)
        {
            pathBlocker.SetActive(false); // Opens the path to the orb

            QuestGiver questGiver = FindObjectOfType<QuestGiver>();
            if (questGiver != null)
            {
                questGiver.SetQuestStage(QuestGiver.QuestStage.FindOrb);
            }
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        if (animator != null)
            animator.SetTrigger("Die");

        if (deathMessageText != null)
            deathMessageText.SetActive(true);

        // Disable all other scripts on this boss GameObject
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
    }

    public void ResetHealth()
    {
        if (isDead) return;

        currentHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }
}
