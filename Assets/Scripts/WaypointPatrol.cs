// WaypointPatrol.cs
// Script by Carlkai
// This script controls AI patrolling behavior between waypoints using NavMeshAgent.
// It also includes vision-based player detection with field of view and distance checks.
// If the player is spotted, a screen fade effect is triggered, and the player is teleported to a designated respawn point.

using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints; // Patrol points to follow in sequence
    public float waitTime = 1f;   // Time to wait at each waypoint

    [Header("Player Stuff")]
    public Transform player; // Reference to player Transform
    private bool isTeleporting = false;
    private CharacterController playerCC; // Cache for CharacterController
    private ThirdPersonController playerController; // Reference to player's movement script

    [Header("Vision Settings")]
    public float detectionRadius = 10f;           // Detection range radius
    public float fieldOfViewAngle = 90f;          // FOV angle for cone of vision
    public LayerMask detectionLayers;             // Layers the AI can detect (e.g. Player)
    public GameObject alertIcon;                  // Alert icon shown when player is detected
    public ScreenFader screenFader;               // Screen fade controller

    [Header("UI Feedback")]
    public GameObject caughtText; // UI shown when player is caught

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool playerInSight = false;
    private bool wasAlerted = false;
    private bool hasPlayedAlertSound = false;
    private bool hasBeenSpotted = false;

    void Start()
    {
        if (player != null)
        {
            playerCC = player.GetComponent<CharacterController>();
            playerController = player.GetComponent<ThirdPersonController>();
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (alertIcon != null)
        {
            alertIcon.SetActive(false);
        }
    }

    void Update()
    {
        DetectPlayer();

        if (!hasBeenSpotted)
        {
            if (!playerInSight)
            {
                if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance < 0.5f))
                {
                    waitTimer += Time.deltaTime;
                    if (waitTimer >= waitTime)
                    {
                        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                        agent.SetDestination(waypoints[currentWaypointIndex].position);
                        waitTimer = 0f;
                    }
                }
            }
            else
            {
                agent.ResetPath();
            }
        }

        HandleAnimationAndAlert();

        if (hasBeenSpotted && screenFader != null && screenFader.IsFullyBlack() && !isTeleporting)
        {
            if (caughtText != null)
                caughtText.SetActive(true);

            if (playerController != null)
                playerController.canMove = false;

            if (playerCC != null)
                playerCC.enabled = false;

            // ✅ Ask RespawnManager where to send the player
            RespawnManager respawner = FindObjectOfType<RespawnManager>();
            if (respawner != null)
            {
                Transform point = respawner.GetCurrentRespawnPoint();
                if (point != null && player != null)
                {
                    player.position = point.position;
                    player.rotation = point.rotation;
                }
            }

            screenFader.FadeToClear();
            isTeleporting = true;
        }
        else if (hasBeenSpotted && isTeleporting && screenFader != null && screenFader.IsFullyClear())
        {
            if (caughtText != null)
                caughtText.SetActive(false);

            if (playerCC != null)
                playerCC.enabled = true;

            if (playerController != null)
                playerController.canMove = true;

            isTeleporting = false;
            hasBeenSpotted = false;
            wasAlerted = false;
            hasPlayedAlertSound = false;

            if (alertIcon != null)
                alertIcon.SetActive(false);

            animator.SetBool("isAlerted", false);
        }
    }

    void HandleAnimationAndAlert()
    {
        if (animator != null)
        {
            float moveSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", Mathf.Clamp(moveSpeed, 0f, 1f));

            if (playerInSight && !hasBeenSpotted)
            {
                hasBeenSpotted = true;

                animator.SetBool("isAlerted", true);

                if (!wasAlerted)
                {
                    wasAlerted = true;

                    if (alertIcon != null)
                        alertIcon.SetActive(true);

                    if (!hasPlayedAlertSound && audioSource != null)
                    {
                        audioSource.Play();
                        hasPlayedAlertSound = true;
                    }
                }

                if (screenFader != null)
                    screenFader.FadeToBlack();
            }
        }
    }

    void DetectPlayer()
    {
        playerInSight = false;

        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRadius)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);

            if (angle <= fieldOfViewAngle / 2f)
            {
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out RaycastHit hit, detectionRadius, detectionLayers))
                {
                    if (hit.transform == player)
                    {
                        playerInSight = true;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 forward = transform.forward;
        Quaternion leftRay = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up);
        Quaternion rightRay = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up);

        Vector3 leftDirection = leftRay * forward;
        Vector3 rightDirection = rightRay * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up, leftDirection * detectionRadius);
        Gizmos.DrawRay(transform.position + Vector3.up, rightDirection * detectionRadius);
    }
}
