using UnityEngine;
public class CubeLanding : MonoBehaviour
{
    private bool hasSpawnedNext = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 🔥 If player hits cube from BELOW or SIDE → Game Over
            if (collision.transform.position.y < transform.position.y + .5f)
            {
                GameManager.instance.LoseLife();
                return;
            }

            // ✅ If player lands from top
            MovingCube movingCube = GetComponent<MovingCube>();
            if (movingCube != null)
            {
                movingCube.enabled = false;
            }

            if (!hasSpawnedNext)
            {
                hasSpawnedNext = true;
                ScoreManager.score++;
                FindObjectOfType<Spawner>().SpawnNextCube();
            }
        }
    }
}