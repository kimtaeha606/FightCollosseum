using UnityEngine;
using UnityEngine.UI;

public class InGameScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Text scoreText;
    [SerializeField] private string scoreFormat = "Score : {0}";

    private void Reset()
    {
        scoreText = GetComponent<Text>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Awake()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<Text>();
        }
    }

    private void OnEnable()
    {
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }

        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
            UpdateScoreText(scoreManager.CurrentScore);
            return;
        }

        UpdateScoreText(0);
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= HandleScoreChanged;
        }
    }

    private void HandleScoreChanged(int score)
    {
        UpdateScoreText(score);
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text = string.Format(scoreFormat, score);
    }
}
