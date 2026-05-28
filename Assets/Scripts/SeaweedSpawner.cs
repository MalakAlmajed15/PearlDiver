using UnityEngine;

public class SeaweedSpawner : MonoBehaviour
{
    public GameObject seaweedPrefab;
    public int numberOfSeaweed = 20;
    public float rangeX = 40f;
    public float rangeZ = 40f;
    public float fixedSpawnY = -8f; // Fixed Y — always on seabed
    public float minScale = 3f;
    public float maxScale = 6f;
    public Vector3 baseRotation = new Vector3(0f, 0f, 0f);

    void Start()
    {
        SpawnSeaweed();
    }

    void SpawnSeaweed()
    {
        if (seaweedPrefab == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 center = player.transform.position;

        for (int i = 0; i < numberOfSeaweed; i++)
        {
            float randomX = center.x + Random.Range(-rangeX, rangeX);
            float randomZ = center.z + Random.Range(-rangeZ, rangeZ);

            // Always use fixedSpawnY s
            Vector3 spawnPos = new Vector3(randomX, fixedSpawnY, randomZ);

            Quaternion rotation = Quaternion.Euler(
                baseRotation.x,
                baseRotation.y + Random.Range(0f, 360f),
                baseRotation.z
            );

            GameObject seaweed = Instantiate(seaweedPrefab, spawnPos, rotation);

            float randomScale = Random.Range(minScale, maxScale);
            seaweed.transform.localScale = Vector3.one * randomScale;
        }

        Debug.Log("Spawned " + numberOfSeaweed + " seaweed!");
    }
}