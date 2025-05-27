// NPCWaveOnProximity.cs
// Controls NPC waving and talking animations based on player proximity and interaction
// NOTES (future Carlkai):
// - NPC waves when player is close and has not interacted yet
// - Stops waving and starts talking when player presses E
// - Stops talking when player walks away
// - Rotates NPC to face the player while in range

using UnityEngine;

public class NPCWaveOnProximity : MonoBehaviour
{
    public float waveDistance = 5f; // Distance within which NPC reacts
    public Transform player;        // Player to detect
    public KeyCode interactKey = KeyCode.E;

    private Animator animator;
    private bool isWaving = false;
    private bool isTalking = false;
    private bool hasInteracted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= waveDistance;

        // NPC faces player smoothly while they're in range
        if (inRange)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        // NPC starts waving if player is near and hasn't talked yet
        if (inRange && !isWaving && !isTalking && !hasInteracted)
        {
            animator.SetBool("isWaving", true);
            isWaving = true;
        }

        // On player interaction, NPC stops waving and starts talking
        if (inRange && Input.GetKeyDown(interactKey) && !isTalking && !hasInteracted)
        {
            animator.SetBool("isWaving", false);
            animator.SetBool("isTalking", true);
            isWaving = false;
            isTalking = true;
            hasInteracted = true;
        }

        // If player leaves, stop talking
        if (!inRange && isTalking)
        {
            animator.SetBool("isTalking", false);
            isTalking = false;
        }
    }
}