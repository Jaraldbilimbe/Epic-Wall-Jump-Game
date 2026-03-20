using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public float distance = 4f;

    private Queue<GameObject> cubes = new Queue<GameObject>();
    private int cubeIndex = 0;

    void Start()
    {
        // Create first 3 cubes
        for (int i = 0; i < 3; i++)
        {
            SpawnCube();
        }
    }

    public void SpawnCube()
    {
        Vector3 pos = new Vector3(0, 0, cubeIndex * distance);
        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity);

        cubes.Enqueue(cube);
        cubeIndex++;
    }

    public void PlayerLanded()
    {
        if (cubes.Count > 3)
        {
            GameObject oldCube = cubes.Dequeue();
            oldCube.GetComponent<Platform>().Fall();
        }
    }
}