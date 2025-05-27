using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Respawn Points")]
    public Transform defaultRespawnPoint;
    public Transform orbRespawnPoint;

    private bool hasCollectedOrb = false;

    // Call this from your orb pickup script
    public void SetOrbCollected()
    {
        hasCollectedOrb = true;
    }

    // Used by WaypointPatrol to know where to teleport the player
    public Transform GetCurrentRespawnPoint()
    {
        return hasCollectedOrb ? orbRespawnPoint : defaultRespawnPoint;
    }
}
