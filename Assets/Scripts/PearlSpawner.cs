using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Pearl Settings")]
    public GameObject pearlPrefab;
    public int numberOfPearls = 7;

    [Header("Golden Pearl")]
    public GameObject goldenPearlPrefab;

    [Header("Pearl Sound")]
    public AudioClip collectSound;

    [Header("Spawn Area (relative to player)")]
    public float spawnRadius = 15f;      // how far from player pearls can spawn
    public float minSpawnDist = 3f;      // minimum distance so they're not inside the player
    public float spawnY = -6f;           // fixed Y depth, or see below

    [Header("World Bounds (safety clamp)")]
    public float worldMinX = -35f;
    public float worldMaxX = 465f;
    public float worldMinZ = -15f;
    public float worldMaxZ = 485f;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("PearlSpawner: no GameObject tagged 'Player' found.");
            return;
        }

        Vector3 playerPos = player.transform.position;

        SpawnPearls(playerPos);
        SpawnGoldenPearl(playerPos);
    }

    void SpawnPearls(Vector3 playerPos)
    {
        if (pearlPrefab == null)
        {
            Debug.LogError("PearlSpawner: pearlPrefab not assigned.");
            return;
        }

        for (int i = 0; i < numberOfPearls; i++)
        {
            Vector3 spawnPos = GetRandomNearbyPosition(playerPos);

            GameObject pearl = Instantiate(pearlPrefab, spawnPos, pearlPrefab.transform.rotation);

            PearlManager pm = pearl.GetComponent<PearlManager>();
            if (pm != null && collectSound != null)
                pm.collectSound = collectSound;
        }

        Debug.Log($"PearlSpawner: spawned {numberOfPearls} pearls near player.");
    }

    void SpawnGoldenPearl(Vector3 playerPos)
    {
        if (goldenPearlPrefab == null)
        {
            Debug.LogWarning("PearlSpawner: no golden pearl prefab assigned.");
            return;
        }

        Vector3 spawnPos = GetRandomNearbyPosition(playerPos);
        Instantiate(goldenPearlPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"PearlSpawner: golden pearl spawned at {spawnPos}");
    }

    Vector3 GetRandomNearbyPosition(Vector3 playerPos)
    {
        // Try up to 10 times to find a valid spot
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            float x = playerPos.x + randomCircle.x;
            float z = playerPos.z + randomCircle.y;

            // Clamp inside terrain
            x = Mathf.Clamp(x, worldMinX, worldMaxX);
            z = Mathf.Clamp(z, worldMinZ, worldMaxZ);

            Vector3 candidate = new Vector3(x, spawnY, z);

            // Make sure it's not too close to the player
            float dist = Vector2.Distance(
                new Vector2(candidate.x, candidate.z),
                new Vector2(playerPos.x, playerPos.z));

            if (dist >= minSpawnDist)
                return candidate;
        }

        // Fallback directly beside player
        return new Vector3(
            Mathf.Clamp(playerPos.x + minSpawnDist, worldMinX, worldMaxX),
            spawnY,
            Mathf.Clamp(playerPos.z, worldMinZ, worldMaxZ));
    }

    void OnDrawGizmosSelected()
    {
        // Show spawn radius in scene view
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnDist);
    }
}