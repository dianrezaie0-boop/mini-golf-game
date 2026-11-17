using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int totalScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int stageScore)
    {
        totalScore += stageScore;

        Debug.Log("Stage Score: " + stageScore);
        Debug.Log("Total Score: " + totalScore);
    }
}