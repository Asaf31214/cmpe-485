using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject prefab;
    public float spawnHeight = 5f;
    public float spawnRange = 3f;
    public float velocityRange = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);
            
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
            
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float velX = Random.Range(-velocityRange, velocityRange);
                float velZ = Random.Range(-velocityRange, velocityRange);
                rb.velocity = new Vector3(velX, 0, velZ);
            }
        }
    }
}