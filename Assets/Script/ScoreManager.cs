using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TMP_Text scoreText;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        scoreText.text = "Score:"+score;
    }
}