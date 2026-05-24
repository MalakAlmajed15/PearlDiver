using UnityEngine;
using System.Collections.Generic;

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
    public float spawnRadius = 15f;
    public float minSpawnDist = 3f;
    public float spawnY = -6f;

    [Header("Pearl Spacing")]
    public float minPearlSpacing = 2.5f;

    [Header("World Bounds (safety clamp)")]
    public float worldMinX = -35f;
    public float worldMaxX = 465f;
    public float worldMinZ = -15f;
    public float worldMaxZ = 485f;

    private List<Vector3> spawnedPositions = new List<Vector3>();

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
            Vector3 spawnPos = GetValidPosition(playerPos, i);
            spawnedPositions.Add(spawnPos);

            GameObject pearl = Instantiate(pearlPrefab, spawnPos, pearlPrefab.transform.rotation);

            PearlManager pm = pearl.GetComponent<PearlManager>();
            if (pm != null && collectSound != null)
                pm.collectSound = collectSound;
        }

        Debug.Log($"PearlSpawner: spawned {numberOfPearls} pearls.");
    }

    void SpawnGoldenPearl(Vector3 playerPos)
    {
        if (goldenPearlPrefab == null)
        {
            Debug.LogWarning("PearlSpawner: no golden pearl prefab assigned.");
            return;
        }

        Vector3 spawnPos = GetValidPosition(playerPos, numberOfPearls);
        spawnedPositions.Add(spawnPos);
        Instantiate(goldenPearlPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"PearlSpawner: golden pearl spawned at {spawnPos}");
    }

    Vector3 GetValidPosition(Vector3 playerPos, int pearlIndex)
    {
        // Pass 1: strict spacing — 50 attempts
        for (int i = 0; i < 50; i++)
        {
            Vector3 candidate = RandomCandidate(playerPos);
            if (IsValidPosition(candidate, playerPos, minPearlSpacing))
                return candidate;
        }

        // Pass 2: relaxed spacing (half) — 50 more attempts
        float relaxedSpacing = minPearlSpacing * 0.5f;
        for (int i = 0; i < 50; i++)
        {
            Vector3 candidate = RandomCandidate(playerPos);
            if (IsValidPosition(candidate, playerPos, relaxedSpacing))
                return candidate;
        }

        // Pass 3: guaranteed unique fallback using index-based angle
        // Places pearls evenly around a circle so they never overlap
        float angle = (360f / (numberOfPearls + 1)) * pearlIndex;
        float rad = angle * Mathf.Deg2Rad;
        float fallbackDist = Mathf.Max(minSpawnDist, minPearlSpacing) + 0.5f;

        float fx = Mathf.Clamp(playerPos.x + Mathf.Cos(rad) * fallbackDist, worldMinX, worldMaxX);
        float fz = Mathf.Clamp(playerPos.z + Mathf.Sin(rad) * fallbackDist, worldMinZ, worldMaxZ);

        Debug.LogWarning($"PearlSpawner: used fallback position for pearl {pearlIndex}");
        return new Vector3(fx, spawnY, fz);
    }

    Vector3 RandomCandidate(Vector3 playerPos)
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float x = Mathf.Clamp(playerPos.x + randomCircle.x, worldMinX, worldMaxX);
        float z = Mathf.Clamp(playerPos.z + randomCircle.y, worldMinZ, worldMaxZ);
        return new Vector3(x, spawnY, z);
    }

    bool IsValidPosition(Vector3 candidate, Vector3 playerPos, float spacing)
    {
        // Must be far enough from player
        float distFromPlayer = Vector2.Distance(
            new Vector2(candidate.x, candidate.z),
            new Vector2(playerPos.x, playerPos.z));

        if (distFromPlayer < minSpawnDist) return false;

        // Must be far enough from every existing pearl
        foreach (Vector3 existing in spawnedPositions)
        {
            float distFromPearl = Vector2.Distance(
                new Vector2(candidate.x, candidate.z),
                new Vector2(existing.x, existing.z));

            if (distFromPearl < spacing) return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnDist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minPearlSpacing);
    }
}