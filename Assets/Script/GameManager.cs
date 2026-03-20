using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioSource gameMusic;
    public AudioSource gameOverMusic;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text livesText;

    [Header("Player Info")]
    public string playerName = "Player";   // Player name

    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Fall Detection")]
    public float fallLimit = -2f;

    private Transform player;
    private KinectJump kinectJump;
    private bool isGameOver = false;

    public bool isGameStarted = false;
    void Awake()
    {
        instance = this;
    }

    public void Playgame()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        kinectJump = player.GetComponent<KinectJump>();

        playerName = PlayerPrefs.GetString("PlayerName", "Player");

        gameOverPanel.SetActive(false);

        currentLives = maxLives;
        UpdateLivesUI();
    }

    void Update()
    {
      if(isGameStarted ){
        if (!isGameOver && player.position.y < fallLimit)
        {
            LoseLife();
        }
      }
    }
    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            ResetPlayerPosition();
        }
    }

    void ResetPlayerPosition()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Try to find the last landed cube to respawn on top of it
        Spawner spawner = FindObjectOfType<Spawner>();
        GameObject lastCube = spawner != null ? spawner.GetLastCube() : null;

        if (lastCube != null)
        {
            float cubeTopY = lastCube.transform.position.y + lastCube.transform.localScale.y / 2f + 1f;
            player.position = new Vector3(lastCube.transform.position.x, cubeTopY, lastCube.transform.position.z);
        }
        else
        {
            // Fallback: respawn at start position above ground
            player.position = new Vector3(0f, 2f, 0f);
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < currentLives; i++)
            {
                hearts += "❤️";
            }
            livesText.text = hearts;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: " + ScoreManager.score;

        // SAVE SCORE TO LEADERBOARD
        SaveScore(PlayerPrefs.GetString("CurrentPlayerName"), ScoreManager.score);

        // Stop background music
        if (gameMusic != null && gameMusic.isPlaying)
        {
            gameMusic.Stop();
        }

        // Play game over music
        if (gameOverMusic != null && !gameOverMusic.isPlaying)
        {
            gameOverMusic.Play();
        }

        // Stop gameplay
        Time.timeScale = 0f;
    }
    void SaveScore(string name, int score)
    {
        for (int i = 0; i < 5; i++)
        {
            int storedScore = PlayerPrefs.GetInt("Score" + i, 0);

            if (score > storedScore)
            {
                for (int j = 4; j > i; j--)
                {
                    PlayerPrefs.SetInt("Score" + j, PlayerPrefs.GetInt("Score" + (j - 1)));
                    PlayerPrefs.SetString("Name" + j, PlayerPrefs.GetString("Name" + (j - 1)));
                }

                PlayerPrefs.SetInt("Score" + i, score);
                PlayerPrefs.SetString("Name" + i, name);

                PlayerPrefs.Save();
                break;
            }
        }
    }

    public void RestartGame()
    {
        ScoreManager.score = 0;
        currentLives = maxLives;

        Time.timeScale = 1f;

        if (kinectJump != null)
        {
            kinectJump.StopKinect();
        }

        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}