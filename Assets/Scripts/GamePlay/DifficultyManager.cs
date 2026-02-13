using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;

    [Header("Growth Per Score")]
    [SerializeField] private float hpPerScore = 0.005f;
    [SerializeField] private float attackPerScore = 0.003f;
    [SerializeField] private float moveSpeedPerScore = 0.001f;
    [SerializeField] private float spawnPerScore = 0.002f;

    [Header("Multiplier Caps")]
    [SerializeField] private float maxHpMultiplier = 4f;
    [SerializeField] private float maxAttackMultiplier = 3f;
    [SerializeField] private float maxMoveSpeedMultiplier = 2f;
    [SerializeField] private float maxSpawnMultiplier = 3f;

    public int CurrentScore { get; private set; }
    public float HpMultiplier { get; private set; } = 1f;
    public float AttackMultiplier { get; private set; } = 1f;
    public float MoveSpeedMultiplier { get; private set; } = 1f;
    public float SpawnMultiplier { get; private set; } = 1f;

    public event Action DifficultyChanged;

    private void Reset()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }

        int initialScore = scoreManager != null ? scoreManager.CurrentScore : 0;
        ApplyScore(initialScore);
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
        }
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
        ApplyScore(score);
    }

    private void ApplyScore(int score)
    {
        CurrentScore = Mathf.Max(0, score);

        HpMultiplier = CalculateMultiplier(CurrentScore, hpPerScore, maxHpMultiplier);
        AttackMultiplier = CalculateMultiplier(CurrentScore, attackPerScore, maxAttackMultiplier);
        MoveSpeedMultiplier = CalculateMultiplier(CurrentScore, moveSpeedPerScore, maxMoveSpeedMultiplier);
        SpawnMultiplier = CalculateMultiplier(CurrentScore, spawnPerScore, maxSpawnMultiplier);
    }

    private static float CalculateMultiplier(int score, float perScore, float maxMultiplier)
    {
        float value = 1f + score * Mathf.Max(0f, perScore);
        return Mathf.Min(value, Mathf.Max(1f, maxMultiplier));
    }
}
