using UnityEngine;

public class CoralSpawner : MonoBehaviour
{
    public GameObject[] coralPrefabs;
    public int numberOfCorals = 15;
    public float rangeX = 40f;
    public float rangeZ = 40f;
    public float spawnY = -8f;
    public float yOffset = -5f;
    public float minScale = 8f;
    public float maxScale = 12f;
    public Vector3 colliderSize = new Vector3(2f, 4f, 2f);
    public Vector3 colliderCenter = new Vector3(0f, 1f, 0f);

    void Start()
    {
        if (coralPrefabs == null || coralPrefabs.Length == 0) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 center = player.transform.position;

        for (int i = 0; i < numberOfCorals; i++)
        {
            GameObject randomCoral = coralPrefabs[Random.Range(0, coralPrefabs.Length)];
            float x = center.x + Random.Range(-rangeX, rangeX);
            float z = center.z + Random.Range(-rangeZ, rangeZ);

            GameObject coral = Instantiate(randomCoral,
                new Vector3(x, spawnY + yOffset, z),
                Quaternion.Euler(-90, Random.Range(0f, 360f), 0));

            coral.transform.localScale = new Vector3(
                Random.Range(minScale, maxScale),
                1f,
                Random.Range(minScale, maxScale)
            );

            BoxCollider bc = coral.GetComponent<BoxCollider>();
            if (bc == null) bc = coral.AddComponent<BoxCollider>();
            bc.size = colliderSize;
            bc.center = colliderCenter;
            bc.isTrigger = false;
        }

        Debug.Log("Spawned " + numberOfCorals + " corals!");
    }
}