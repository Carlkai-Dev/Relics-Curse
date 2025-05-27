// FireballImpact.cs
// Controls fireball projectile behavior, homing movement, collision detection, explosion effect, sound, and player respawn.
// 🔴 NOTES (future Carlkai):
// - This fireball tracks the player using a Rigidbody and Lerp for turning
// - Explodes on impact unless it hits the Boss
// - If it hits the player, it teleports them to a defined respawn point

using System.Collections;
using UnityEngine;

public class FireballImpact : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;       // Where player respawns after hit
    public Transform player;             //  Player to affect on impact

    [Header("Explosion Effect")]
    public GameObject explosionEffect;   // Particle system prefab
    public float destroyDelay = 2f;      // Time before fireball is destroyed

    [Header("Curve Effect")]
    public Transform target;             // Target to home toward
    public float speed = 10f;            // Move speed
    public float turnSpeed = 2f;         // How fast it turns toward player
    private Rigidbody rb;

    [Header("Sound")]
    public AudioClip explosionSound;     //  Sound when exploding

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //  Launch toward target immediately
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            rb.velocity = dir * speed;
        }
    }

    void FixedUpdate()
    {
        if (target == null || rb == null) return;

        //  Smooth turning toward target
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 newDirection = Vector3.Lerp(rb.velocity.normalized, directionToTarget, Time.fixedDeltaTime * turnSpeed);
        rb.velocity = newDirection.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        //  Ignore boss collisions (friendly fire prevention)
        if (collision.collider.CompareTag("Boss")) return;

        //  Visual explosion
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        //  Sound effect
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Destroy(gameObject, destroyDelay); //  Cleanup fireball

        // 🧍‍♂️ Player hit = teleport to respawn
        if (collision.collider.CompareTag("Player") && player != null && respawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false; // Disable to safely reposition
            player.position = respawnPoint.position;
            player.rotation = respawnPoint.rotation;
            if (cc != null) cc.enabled = true;  // Re-enable control
        }
    }
}
