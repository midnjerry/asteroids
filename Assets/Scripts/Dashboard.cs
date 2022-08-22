using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    public Text nextGoal;
    public Text livesText;
    public Text gameOver;

    private void Start()
    {
        updateScore(0);
        updateLives(0);
        updateGoal(10000);
    }

    public void updateLives(int lives)
    {
        string text = "";
        for (int i = 0; i < lives; i++)
        {
            text += "*";
        }
        livesText.text = text;
    }

    public void updateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void updateHighScore(int score, string intials)
    {
        highScoreText.text = score + " " + intials;
    }

    public void updateGoal(int goal)
    {
        nextGoal.text = "BONUS AT " + goal;
    }

    public void setGameOver(bool isGameOver)
    {
        if (isGameOver)
        {
            gameOver.text = "GAME OVER";
        }
        else
        {
            gameOver.text = "";
        }
    }

}
