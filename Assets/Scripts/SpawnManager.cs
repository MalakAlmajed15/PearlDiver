using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  ENEMY PREFABS
    // ─────────────────────────────────────────
    [Header("Enemy Prefabs")]
    public GameObject jellyfishPrefab;
    public GameObject crabPrefab;
    public GameObject electricEelPrefab;

    // ─────────────────────────────────────────
    //  SIZE SCALING  (1 = default prefab size)
    // ─────────────────────────────────────────
    [Header("Enemy Size Multipliers")]
    [Range(0.1f, 5f)] public float jellyfishScale = 1f;
    [Range(0.1f, 5f)] public float crabScale = 1f;
    [Range(0.1f, 5f)] public float eelScale = 1f;

    // ─────────────────────────────────────────
    //  SPAWN TIMING
    // ─────────────────────────────────────────
    [Header("Spawn Timing (seconds)")]
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;

    // ─────────────────────────────────────────
    //  TERRAIN / WORLD BOUNDS
    //  Enemies will never be placed outside
    //  these limits or below the floor.
    // ─────────────────────────────────────────
    [Header("World Bounds")]
    public float worldMinX = -35f;   // terrain left edge
    public float worldMaxX = 965f;   // terrain right edge
    public float worldMinY = -10f;   // terrain floor
    public float worldMaxY = 10f;   // OceanSurface Y ← enemies won't breach the surface
    public float worldMinZ = -15f;   // terrain front edge
    public float worldMaxZ = 985f;   // terrain back edge

    // How close to the player we can spawn (avoids instant hits)
    [Header("Spawn Radius Around Player")]
    public float minSpawnDist = 8f;
    public float maxSpawnDist = 20f;

    // ─────────────────────────────────────────
    //  LEVEL  (set this from your Game Manager
    //          or change it in the Inspector)
    // ─────────────────────────────────────────
    [Header("Current Level (1, 2, 4 or 5)")]
    public int currentLevel = 1;

    // ─────────────────────────────────────────
    //  INTERNALS
    // ─────────────────────────────────────────
    private GameObject[] _pool;   // active prefab choices for this level

    // =========================================
    void Start()
    {
        BuildPool();
        ScheduleNextSpawn();
    }

    // =========================================
    //  Build the prefab list that matches the
    //  level rules from the design table.
    // =========================================
    void BuildPool()
    {
        switch (currentLevel)
        {
            case 1:   // Shallow Reef  – Jellyfish only
                _pool = new[] { jellyfishPrefab };
                break;

            case 2:   // Coral Garden  – Jellyfish + Crabs
                _pool = new[] { jellyfishPrefab, crabPrefab };
                break;

            case 4:   // Deep Cave     – All types
            case 5:   // Treasure Cove – All types
                _pool = new[] { jellyfishPrefab, crabPrefab, electricEelPrefab };
                break;

            default:
                Debug.LogWarning($"SpawnManager: unknown level {currentLevel}. Defaulting to Jellyfish only.");
                _pool = new[] { jellyfishPrefab };
                break;
        }
    }

    // =========================================
    void ScheduleNextSpawn()
    {
        float delay = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke(nameof(SpawnEnemy), delay);
    }

    // =========================================
    void SpawnEnemy()
    {
        if (_pool == null || _pool.Length == 0)
        {
            Debug.LogError("SpawnManager: pool is empty – assign prefabs in the Inspector.");
            return;
        }

        // Pick a random enemy type for this level
        int idx = Random.Range(0, _pool.Length);
        GameObject prefab = _pool[idx];

        // Find player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("SpawnManager: no GameObject tagged 'Player' found.");
            ScheduleNextSpawn();
            return;
        }

        // Try to find a valid spawn position (max 10 attempts)
        Vector3 spawnPos;
        bool found = TryGetSpawnPosition(player.transform.position, out spawnPos);

        if (!found)
        {
            // Fall back: use a random world position that is within bounds
            spawnPos = new Vector3(
                Random.Range(worldMinX, worldMaxX),
                Random.Range(worldMinY, worldMaxY),
                Random.Range(worldMinZ, worldMaxZ)
            );
        }

        // Instantiate and apply the correct scale
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        ApplyScale(enemy, prefab);

        ScheduleNextSpawn();
    }

    // =========================================
    //  Try to find a spawn point that is:
    //    • inside world bounds
    //    • between minSpawnDist and maxSpawnDist from the player
    // =========================================
    bool TryGetSpawnPosition(Vector3 playerPos, out Vector3 result)
    {
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Random direction, random distance in the allowed ring
            Vector2 dir2D = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDist, maxSpawnDist);

            float candidateX = playerPos.x + dir2D.x * dist;
            float candidateY = playerPos.y + Random.Range(-5f, 5f);
            float candidateZ = playerPos.z + dir2D.y * dist;

            // Clamp strictly inside world bounds
            candidateX = Mathf.Clamp(candidateX, worldMinX, worldMaxX);
            candidateY = Mathf.Clamp(candidateY, worldMinY, worldMaxY);
            candidateZ = Mathf.Clamp(candidateZ, worldMinZ, worldMaxZ);

            Vector3 candidate = new Vector3(candidateX, candidateY, candidateZ);

            // Accept if it still respects the minimum distance after clamping
            if (Vector3.Distance(candidate, playerPos) >= minSpawnDist)
            {
                result = candidate;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    // =========================================
    //  Apply the per-type scale multiplier
    // =========================================
    void ApplyScale(GameObject enemy, GameObject prefab)
    {
        float multiplier = 1f;

        if (prefab == jellyfishPrefab) multiplier = jellyfishScale;
        else if (prefab == crabPrefab) multiplier = crabScale;
        else if (prefab == electricEelPrefab) multiplier = eelScale;

        enemy.transform.localScale = prefab.transform.localScale * multiplier;
    }

    // =========================================
    //  Call this from your Game Manager when
    //  the level changes (e.g. LoadLevel(2))
    // =========================================
    public void SetLevel(int level)
    {
        currentLevel = level;
        BuildPool();
    }

    // =========================================
    //  Draw bounds gizmo in Scene view
    // =========================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(
            (worldMinX + worldMaxX) / 2f,
            (worldMinY + worldMaxY) / 2f,
            (worldMinZ + worldMaxZ) / 2f
        );
        Vector3 size = new Vector3(
            worldMaxX - worldMinX,
            worldMaxY - worldMinY,
            worldMaxZ - worldMinZ
        );
        Gizmos.DrawWireCube(center, size);
    }
}