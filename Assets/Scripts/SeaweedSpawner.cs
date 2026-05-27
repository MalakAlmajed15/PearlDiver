using UnityEngine;

public class SeaweedSpawner : MonoBehaviour
{
    [Header("Seaweed Settings")]
    public GameObject seaweedPrefab;
    public int numberOfSeaweed = 20;

    [Header("Spawn Area")]
    public float rangeX = 40f;
    public float rangeZ = 40f;
    public float spawnY = -7f;
    public float yOffset = 0f;

    [Header("Random Size")]
    public float minScale = 3f;
    public float maxScale = 6f;

    [Header("Rotation Fix")]
    public Vector3 baseRotation = new Vector3(0f, 0f, 0f);

    void Start()
    {
        SpawnSeaweed();
    }

    void SpawnSeaweed()
    {
        if (seaweedPrefab == null)
        {
            Debug.LogWarning("No seaweed prefab assigned!");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No Player found!");
            return;
        }

        Vector3 center = player.transform.position;

        for (int i = 0; i < numberOfSeaweed; i++)
        {
            float randomX = center.x + Random.Range(-rangeX, rangeX);
            float randomZ = center.z + Random.Range(-rangeZ, rangeZ);

            // spawnY + yOffset controls up and down
            Vector3 spawnPos = new Vector3(randomX, spawnY + yOffset, randomZ);

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