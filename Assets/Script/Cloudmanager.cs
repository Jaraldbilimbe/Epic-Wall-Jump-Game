using UnityEngine;
using System.Collections.Generic;

public class CloudManager : MonoBehaviour
{
    public GameObject cloudPrefab;
    public float distance = 4f;

    private Queue<GameObject> clouds = new Queue<GameObject>();
    private int cloudIndex = 0;

    void Start()
    {
        // Spawn initial 2 clouds
        for (int i = 0; i < 2; i++)
        {
            SpawnCloud();
        }
    }

    public void SpawnCloud()
    {
        Vector3 pos = new Vector3(0, 0, cloudIndex * distance);

        GameObject cloud = Instantiate(cloudPrefab, pos, Quaternion.identity);

        clouds.Enqueue(cloud);
        cloudIndex++;
    }

    public void PlayerLanded()
    {
        if (clouds.Count > 2)
        {
            GameObject oldCloud = clouds.Dequeue();

            Platform c = oldCloud.GetComponent<Platform>();
            if (c != null)
            {
                c.Fall();
            }
        }
    }
}