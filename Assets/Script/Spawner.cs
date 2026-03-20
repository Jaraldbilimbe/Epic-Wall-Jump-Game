using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Spawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform player;
    public Transform spawner1;
    public Transform spawner2;

    [Header("Keep 3 cubes")]
    [Tooltip("Y position below which the oldest cube is destroyed (below camera)")]
    public float destroyBelowY = -15f;

    private List<GameObject> cubes = new List<GameObject>();
    private bool spawnFromSpawner1 = true;

    void Start()
    {
         //SpawnNextCube();      
    }

    public void SpawnNextCube()
    {
        Vector3 spawnPos;

        if (spawnFromSpawner1)
            spawnPos = spawner1 != null ? spawner1.position : new Vector3(-8f, 5f, player.position.z);
        else
            spawnPos = spawner2 != null ? spawner2.position : new Vector3(8f, 5f, player.position.z);

        GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        float cubeHeight = cube.transform.localScale.y;
        Vector3 targetPos;

        // FIRST CUBE -> GO TO GROUND
        if (cubes.Count == 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(player.position, Vector3.down, out hit, 20f))
            {
                targetPos = new Vector3(
                    player.position.x,
                    hit.point.y + cubeHeight / 2f,
                    player.position.z
                );
            }
            else
            {
                targetPos = new Vector3(
                    player.position.x,
                    cubeHeight / 2f,
                    player.position.z
                );
            }
        }
        else
        {
            // STACK ABOVE LAST CUBE
            GameObject lastCube = cubes[cubes.Count - 1];

            targetPos = new Vector3(
                player.position.x,
                lastCube.transform.position.y + cubeHeight,
                player.position.z
            );
        }

        cube.GetComponent<MovingCube>().SetTarget(targetPos);
        cubes.Add(cube);

        spawnFromSpawner1 = !spawnFromSpawner1;

        // When we have more than 3 cubes, move the oldest one down off camera and destroy it (always keep 3 for player)
        if (cubes.Count > 3)
        {
            GameObject cubeToRemove = cubes[0];
            cubes.RemoveAt(0);

            Vector3 downTarget = new Vector3(
                cubeToRemove.transform.position.x,
                destroyBelowY,
                cubeToRemove.transform.position.z
            );
            MovingCube moving = cubeToRemove.GetComponent<MovingCube>();
            if (moving != null)
                moving.SetTarget(downTarget);

            StartCoroutine(DestroyCubeAfterMovingDown(cubeToRemove));
        }
    }

    IEnumerator DestroyCubeAfterMovingDown(GameObject cube)
    {
        if (cube == null) yield break;
        float timeout = 5f;
        float elapsed = 0f;
        while (cube != null && cube.transform.position.y > destroyBelowY + 0.5f && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (cube != null)
            Destroy(cube);
    }

    public GameObject GetLastCube()
    {
        if (cubes.Count == 0) return null;
        return cubes[cubes.Count - 1];
    }
    IEnumerator FixStack()
    {
        yield return new WaitForSeconds(0.1f);

        if (cubes.Count < 2) yield break;

        GameObject bottomCube = cubes[0];
        GameObject topCube = cubes[1];

        float cubeHeight = bottomCube.transform.localScale.y;

        // Bottom cube falls to ground
        Rigidbody rb = bottomCube.GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Wait until cube lands
        yield return new WaitForSeconds(0.5f);

        // Move top cube above the fallen cube
        Vector3 newTarget = new Vector3(
            player.position.x,
            bottomCube.transform.position.y + cubeHeight,
            player.position.z
        );

        topCube.GetComponent<MovingCube>().SetTarget(newTarget);
    }
}   