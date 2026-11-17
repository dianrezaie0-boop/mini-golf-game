using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinManager : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            scoreText.text = "Final Score: " + ScoreManager.Instance.totalScore;
        }
    }

    public void RestartGame()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.totalScore = 0;

        SceneManager.LoadScene("Level1");
    }

    public void MainMenu()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.totalScore = 0;

        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();

        Debug.Log("Exit Game");
    }
}