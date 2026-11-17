using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShotManager : MonoBehaviour
{
    public TMP_Text shotText;

    public int maxShots = 3;

    private int currentShots;

    public GameObject gameOverPanel;


    void Start()
    {
        currentShots = maxShots;
        UpdateUI();
    }

    public void UseShot()
    {
        currentShots--;

        if (currentShots < 0)
            currentShots = 0;

        UpdateUI();

        if (currentShots == 0)
        {
            StartCoroutine(CheckGameOver());
        }

    }

    void UpdateUI()
    {
        shotText.text = "Shots: " + currentShots;
    }

    public bool HasShot()
    {
        return currentShots > 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator CheckGameOver()
    {
        BallController ball = FindObjectOfType<BallController>();

        if (ball == null)
            yield break;

        while (ball.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);

        if (ball.levelCompleted)
            yield break;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public int GetRemainingShots()
    {
        return currentShots;
    }
}