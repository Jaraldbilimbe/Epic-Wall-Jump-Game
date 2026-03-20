using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public Transform player;

    public float spawnHeight = 6f;     // height above player
    public float leftX = -2.5f;
    public float rightX = 2.5f;
    public Transform leftTransform;
    public Transform righttransform;
    public float spawnInterval = 1.2f;

    private bool spawnLeft = true;

    void Start()
    {
        InvokeRepeating(nameof(SpawnTile), 0f, spawnInterval);
    }

    void SpawnTile()
    {
        float xPos = spawnLeft ? leftX : rightX;
         Vector3 _spawnPOS=spawnLeft?leftTransform.position:righttransform.position; 
        // Vector3 spawnPos = new Vector3(
        //     xPos,
        //     player.position.y + spawnHeight,
        //     0f
        // );

        GameObject tempCloud= Instantiate(cloudPrefab, _spawnPOS, Quaternion.identity);
        //tempCloud.transform.SetParent(transform);
        spawnLeft = !spawnLeft;
    }
}