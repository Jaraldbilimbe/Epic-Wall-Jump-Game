using UnityEngine;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    public GameObject homePanel;
    public TMP_InputField playerNameInput;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;

    public Spawner spawner;
    public AudioSource gameMusic;
    public KinectJump kinectJump;

    public GameObject scorecardPanel;
    public ScorecardUI scorecardUI;
    public GameObject Signup;

    public GameObject maingameobjects;

    public GameObject initialbackground;
    public GameManager gameManager;
    void Start()
    {
        //Time.timeScale = 0f;
        homePanel.SetActive(true);

        scoreText.gameObject.SetActive(false); // Hide score at start
    }

    public void StartGame()
{
    Playbutton();
    maingameobjects.SetActive(true);
    gameManager.Playgame();
    gameManager.isGameStarted=true;
    string playerName = playerNameInput.text;

    // If no name entered
    if (playerName == "")
    {
        playerName = "Player";
    }

    // Save player name
    PlayerPrefs.SetString("CurrentPlayerName", playerName);
    PlayerPrefs.Save();

    // Show player name text
    playerNameText.text = "PLAYER: " + playerName;

    // Hide home panel
    homePanel.SetActive(false);

    // Hide player name during gameplay
    //playerNameText.gameObject.SetActive(false);

    // Show score UI
    scoreText.gameObject.SetActive(true);

    // Start game time
    Time.timeScale = 1f;

    // Enable kinect jumping
    if (kinectJump != null)
    {
        kinectJump.enabled = true;
    }

    // Play music
    if (gameMusic != null)
    {
        gameMusic.Play();
    }

    // Spawn first cube
    if (spawner != null)
    {
        spawner.SpawnNextCube();
    }
}

    // Called when Scorecard button is clicked on home screen
    public void OpenScorecardFromHome()
    {
        homePanel.SetActive(false);

        if (scorecardPanel != null)
        {
            scorecardPanel.SetActive(true);
        }

        if (scorecardUI != null)
        {
            scorecardUI.UpdateLeaderboard();
        }
    }

    // Called when Back button is clicked on scorecard panel
    public void CloseScorecardToHome()
    {
        if (scorecardPanel != null)
        {
            scorecardPanel.SetActive(false);
        }

        homePanel.SetActive(true);
    }

    public void Playbutton()
    {
       Signup.SetActive(true);
       initialbackground.SetActive(false);
    }
}