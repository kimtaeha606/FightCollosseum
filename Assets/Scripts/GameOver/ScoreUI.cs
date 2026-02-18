using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private string scoreFormat = "Score : {0}";

    private void Reset()
    {
        scoreText = GetComponent<Text>();
    }

    private void Awake()
    {
        if (scoreText == null)
        {
            return;
        }

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        int score = scoreManager != null ? scoreManager.CurrentScore : 0;
        scoreText.text = string.Format(scoreFormat, score);
    }
}
