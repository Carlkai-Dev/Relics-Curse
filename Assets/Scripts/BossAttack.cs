// BossCaster.cs
// Controls the boss's fireball casting logic, detection radius, cooldown, HP UI, and damage upon casting.
// 🟣 NOTES (future self):
// - Boss triggers DefeatWizard quest stage when player enters range
// - Casts fireball every X seconds, loses 1 HP per cast
// - Uses a coroutine to sync animation, audio, and projectile
// - Resets casting and health when player leaves detection range

using System.Collections;
using UnityEngine;

public class BossCaster : MonoBehaviour
{
    [Header("References")]
    public Transform player;                     // Who to target with fireballs
    public GameObject fireballPrefab;            //  Fireball prefab to spawn
    public Transform castPoint;                  //  Where to spawn fireballs from
    public Animator animator;                    //  Boss casting animation
    public Transform respawnPoint;               //  Where to respawn player if hit

    [Header("Casting Settings")]
    public float castCooldown = 4f;              //  Time between casts
    public float detectionRange = 12f;           //  Range to detect player
    public float FireballSpeed = 10f;            //  Speed of fireball
    public int maxCasts = 4;                     //  Boss dies after 4 casts

    [Header("Boss UI")]
    public GameObject bossHPUI;                  // Optional HP bar to show when active

    [Header("Audio")]
    public AudioSource castingAudioSource;       //  Sound when boss casts

    private bool playerInRangeLastFrame = false;
    private float cooldownTimer = 0f;
    private int castCount = 0;
    private bool isCasting = false;
    private bool hasTriggeredDefeatWizardStage = false;

    void Update()
    {
        if (castCount >= maxCasts || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isInRange = distance <= detectionRange;

        // 🎯 Player enters detection range
        if (isInRange && !playerInRangeLastFrame)
        {
            if (!hasTriggeredDefeatWizardStage)
            {
                hasTriggeredDefeatWizardStage = true;

                QuestGiver questGiver = FindObjectOfType<QuestGiver>();
                if (questGiver != null)
                    questGiver.SetQuestStage(QuestGiver.QuestStage.DefeatWizard);
            }

            bossHPUI?.SetActive(true);
        }

        // 🚪 Player leaves detection range
        if (!isInRange && playerInRangeLastFrame)
        {
            bossHPUI?.SetActive(false);

            BossHealth bh = GetComponent<BossHealth>();
            if (bh != null)
                bh.ResetHealth();

            castCount = 0;
            animator.SetBool("Cast", false);
            isCasting = false;
        }

        playerInRangeLastFrame = isInRange;

        // 🎬 Begin casting logic if player is in range
        if (isInRange && !isCasting)
        {
            // Boss turns to face the player
            Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookPos);

            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= castCooldown)
            {
                StartCoroutine(CastSpell());
                cooldownTimer = 0f;
            }
        }
    }

    IEnumerator CastSpell()
    {
        isCasting = true;
        animator.SetBool("Cast", true);

        if (castingAudioSource != null)
            castingAudioSource.Play();

        yield return new WaitForSeconds(0.5f); // Delay to match cast animation timing

        // 🔥 Spawn fireball and assign homing logic
        if (fireballPrefab != null && castPoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, castPoint.position, Quaternion.identity);

            FireballImpact homing = fireball.GetComponent<FireballImpact>();
            if (homing != null)
            {
                homing.target = player;
                homing.speed = FireballSpeed;
                homing.player = player;
                homing.respawnPoint = respawnPoint;
            }

            GetComponent<BossHealth>().TakeDamage(1); // Lose HP per cast
        }

        castCount++;

        yield return new WaitForSeconds(0.8f); // Cooldown before next cast

        animator.SetBool("Cast", false);
        if (castingAudioSource != null)
            castingAudioSource.Stop();

        isCasting = false;
    }

    // Shows red radius gizmo in editor for debugging detection range
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
