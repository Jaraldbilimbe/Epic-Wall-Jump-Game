using UnityEngine;
using TMPro;

public class ScorecardUI : MonoBehaviour
{
    public GameObject scorecardPanel;
    public GameObject gameOverPanel;

    public TMP_Text[] rankTexts;
    public TMP_Text[] playerNameTexts;
    public TMP_Text[] scoreTexts;

    public void OpenScorecard()
    {
        // Hide Game Over panel
        gameOverPanel.SetActive(false);

        // Show Scorecard panel
        scorecardPanel.SetActive(true);

        UpdateLeaderboard();
    }

    public void CloseScorecard()
    {
        // Hide scorecard
        scorecardPanel.SetActive(false);

        // Show Game Over panel again
        gameOverPanel.SetActive(true);
    }

    public void UpdateLeaderboard()
    {
        for (int i = 0; i < 5; i++)
        {
            string name = PlayerPrefs.GetString("Name" + i, "---");
            int score = PlayerPrefs.GetInt("Score" + i, 0);

            rankTexts[i].text = (i + 1).ToString();
            playerNameTexts[i].text = name;
            scoreTexts[i].text = score.ToString();
        }
    }
    // CLEAR ALL SCORES
    public void ClearLeaderboard()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.DeleteKey("Name" + i);
            PlayerPrefs.DeleteKey("Score" + i);
        }

        PlayerPrefs.Save();

        UpdateLeaderboard();
    }
}